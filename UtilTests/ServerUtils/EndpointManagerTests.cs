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

            List<IEndpoint> epList = new List<IEndpoint>();

            SemaphoreSlim sem = new(1);

            EventHandler<EndpointChangedEventArgs> connectionChanged = (sender, e) =>
            {
                Assert.AreEqual(expectedType, e.Type);

                sem.Wait();
                if (e.Type == EndpointEventType.Connect)
                {
                    epList.Add(e.Endpoint);
                }
                else
                {
                    Assert.IsTrue(epList.Remove(e.Endpoint));
                }

                if (local)
                {
                    localDisconnectEp = e.Endpoint;
                }

                TestUtils.AssertTask(barrier.SignalAsync());
                sem.Release();
            };
            endpointManager.EndpointConnectionChanged += connectionChanged;

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

            // disconnect client (extern)
            expectedType = EndpointEventType.Disconnect;
            barrier = new(1, false);
            clientRemoteDisconnect.Close();
            TestUtils.AssertTask(barrier.WaitAsync());
            Assert.AreEqual(4, endpointManager.ConnectedEndpoints.Count);
            Assert.AreEqual(4, epList.Count);
            CollectionAssert.AreEquivalent(epList, endpointManager.ConnectedEndpoints);
            CollectionAssert.Contains(epList, localDisconnectEp);
            CollectionAssert.Contains(endpointManager.ConnectedEndpoints, localDisconnectEp);

            // disconnect client (local, null)
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
