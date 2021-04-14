using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using NetworkUtils.Tcp_Client_Listener;
using ServerUtils;
using ServerUtils.Endpoint_Manager;
using SyncUtils.Barrier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using UtilTests.TestUtilities;

namespace UtilTests.ServerUtils
{
    [TestClass]
    public class EndpointManagerTests
    {
        [TestMethod]
        public void TestFailingConnectDisconnect()
        {
            const int PORT = 1234;
            const int COUNT = 1;

            TcpClientListener listener = new TcpClientListener();
            EndpointManager endpointManager = new();

            listener.StartListeningForConnections(PORT);
            listener.ConnectingClientBlock.LinkTo(endpointManager.IncomingClientBlock);

            //  connect (multiple connects, handler has fix blocking time)
            //  connect (handler throws exception)
            //  connect (handler timeout)
            //  disconnect client
            //  disconnect all 
            //  disconnect (multiple disconnects) 
            //  disconnect client (handler throws exception)
            //  disconnect client (handler timeout) 
            //  disconnect (multiple connects, handler has fix blocking time)
            //  disconnect while connect is still taking place
            //

            AsyncBarrier barrier = new(COUNT, false);
            SemaphoreSlim sem = new(1, 1);

            int nextType = 0;
            int n = 0;
            endpointManager.EndpointConnectedHandler = async ep =>
            {
                await sem.WaitAsync();
                int t = n++;
                int type = nextType;
                nextType = (nextType + 1) % 3;
                sem.Release();

                if (type == 0 && t < 2)
                {
                    throw new Exception();
                }
                else if (type == 1)
                {
                    while (true) ;
                }
                else if(type == 2)
                {
                    await barrier.SignalAsync();                    
                    await Task.Delay(300);
                }
            };

            //  connect (multiple connects, handler has fix blocking time)
            //  connect (handler throws exception)
            //  connect (handler timeout)
            List<Task<PacketSocket>> tasks = new();
            for(int i = 0; i < 3 * COUNT; i++)
            {
                tasks.Add(ConnectSocketAsync(PORT));
            }
            TestUtils.AssertTask(Task.WhenAll(tasks), 20000);
            TestUtils.AssertTask(barrier.WaitAsync());
            List<PacketSocket> sockets = new();
            for(int i = 0; i < COUNT; i++)
            {   
                sockets.Add(tasks[2 + i * 3].Result);
            }
            Assert.IsFalse(sockets.Where(s => !s.IsConnected || !s.IsReceiving).Any());



        }
        private Task<PacketSocket> ConnectSocketAsync(int port)
        {
            return Task.Run(() =>
            {
                TcpClient client = new TcpClient("localhost", port);
                return new PacketSocket(client);
            });
        }

        [TestMethod]
        public void TestBasicFunctionality()
        {
            const int PORT = 1234;

            EndpointManager endpointManager = new();

            BufferBlock<TcpClient> block = new();

            block.LinkTo(endpointManager.IncomingClientBlock);

            using TcpClientListener listener = new TcpClientListener();
            listener.StartListeningForConnections(PORT);
            listener.ConnectingClientBlock.LinkTo(endpointManager.IncomingClientBlock);

            //
            // add unconnected client
            // add client -> multiple clients
            // disconnect client (extern)
            // disconnect client (local, null)
            // disconnect client (local, unregistered)
            // disconnect client (local)
            // add 2 clients
            // send message via client 1
            // send message via client 2
            //

            AsyncBarrier barrier = null;

            EndpointEventType expectedType = new();

            TcpClient clientUnconnected = new();
            TcpClient client1 = new();
            TcpClient client2 = new();
            TcpClient client3 = new();
            TcpClient clientRemoteDisconnect = new();
            TcpClient clientLocalDisconnect = new();
            IEndpoint ep = new Endpoint(new PacketSocket(clientUnconnected));
            IEndpoint localDisconnectEp = null;
            bool local = false;

            bool remoteDisconnect = false;

            List<IEndpoint> epList = new List<IEndpoint>();

            SemaphoreSlim sem = new(1);

            endpointManager.EndpointConnectedHandler = async ep =>
            {
                Assert.IsTrue(expectedType == EndpointEventType.Connect);

                await sem.WaitAsync();
                epList.Add(ep);

                if (local)
                {
                    localDisconnectEp = ep;
                }

                TestUtils.AssertTask(barrier.SignalAsync());
                sem.Release();
            };
            endpointManager.EndpointDisconnectedHandler = async (ep, remote) =>
            {
                Assert.AreEqual(remoteDisconnect, remote);
                Assert.IsTrue(expectedType == EndpointEventType.Disconnect);

                await sem.WaitAsync();

                Assert.IsTrue(epList.Remove(ep));

                TestUtils.AssertTask(barrier.SignalAsync());
                sem.Release();
            };

            expectedType = EndpointEventType.Connect;
            barrier = new(4, false);

            // add unconnected client
            TestUtils.AssertTask(block.SendAsync(clientUnconnected));

            var tasks = new List<Task>();

            // add client -> multiple clients
            tasks.Add(Task.Run(() => client1.Connect("localhost", PORT)));
            tasks.Add(Task.Run(() => client2.Connect("localhost", PORT)));
            tasks.Add(Task.Run(() => client3.Connect("localhost", PORT)));
            tasks.Add(Task.Run(() => clientRemoteDisconnect.Connect("localhost", PORT)));
            TestUtils.AssertTask(barrier.WaitAsync(), 3000);

            local = true;
            barrier = new(1, false);
            tasks.Add(Task.Run(() => clientLocalDisconnect.Connect("localhost", PORT)));
            TestUtils.AssertTask(barrier.WaitAsync(), 3000);
            local = false;

            CollectionAssert.Contains(epList, localDisconnectEp);
            CollectionAssert.Contains(endpointManager.ConnectedEndpoints, localDisconnectEp);
            Assert.AreEqual(5, endpointManager.ConnectedEndpoints.Count);
            CollectionAssert.AreEquivalent(epList, endpointManager.ConnectedEndpoints);
            Assert.IsNotNull(localDisconnectEp);

            // disconnect client (remote)
            expectedType = EndpointEventType.Disconnect;
            remoteDisconnect = true;
            barrier = new(1, false);
            clientRemoteDisconnect.Close();
            TestUtils.AssertTask(barrier.WaitAsync());
            Assert.AreEqual(4, endpointManager.ConnectedEndpoints.Count);
            Assert.AreEqual(4, epList.Count);
            CollectionAssert.AreEquivalent(epList, endpointManager.ConnectedEndpoints);
            CollectionAssert.Contains(epList, localDisconnectEp);
            CollectionAssert.Contains(endpointManager.ConnectedEndpoints, localDisconnectEp);

            // disconnect client (local, null)
            remoteDisconnect = false;
            TestUtils.AssertException<ArgumentNullException>(endpointManager.DisconnectEndpointAsync(null));

            // disconnect client (local, unregistered)
            TestUtils.AssertException<ArgumentException>(endpointManager.DisconnectEndpointAsync(ep));

            CollectionAssert.AreEquivalent(epList, endpointManager.ConnectedEndpoints);
            // disconnect client (local)
            barrier = new(1, false);
            CollectionAssert.Contains(endpointManager.ConnectedEndpoints, localDisconnectEp);
            TestUtils.AssertTask(endpointManager.DisconnectEndpointAsync(localDisconnectEp));
            TestUtils.AssertTask(barrier.WaitAsync());
            CollectionAssert.AreEquivalent(epList, endpointManager.ConnectedEndpoints);
            CollectionAssert.DoesNotContain(endpointManager.ConnectedEndpoints, localDisconnectEp);

            barrier = new(1, false);

            byte[] buffer = Encoding.ASCII.GetBytes("Hello World!");

            ActionBlock<RawMessage> aB = new(m =>
            {
                CollectionAssert.AreEqual(buffer, m.Data);
                _ = barrier.SignalAsync();
            });

            foreach (var e in endpointManager.ConnectedEndpoints)
            {
                Assert.IsTrue(e.Socket.IsConnected);
            }
            Assert.IsTrue(client1.Connected);

            PacketSocket socket1 = new PacketSocket(client1);


            Assert.IsTrue(socket1.IsReceiving);

            Assert.IsTrue(socket1.IsConnected);


            endpointManager.RawMessageBlock.LinkTo(aB);

            Assert.IsTrue(TestUtils.AssertTask(socket1.SendAsync(buffer)));
            TestUtils.AssertTask(barrier.WaitAsync());
        }

    }
}
