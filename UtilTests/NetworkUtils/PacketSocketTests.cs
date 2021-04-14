using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Socket;
using NetworkUtils.Tcp_Client_Listener;
using SyncUtils.Barrier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using UtilTests.TestUtilities;

namespace UtilTests.NetworkUtils
{
    [TestClass]
    public class PacketSocketTests
    {
        private static void ResetSockets(out PacketSocket server, out PacketSocket client)
        {
            const int Port = 123;

            TaskCompletionSource<TcpClient> serverTask = new();

            var actionBlock = new ActionBlock<TcpClient>(tcp =>
            {
                serverTask.SetResult(tcp);
            });

            using TcpClientListener listener = new();
            listener.ConnectingClientBlock.LinkTo(actionBlock);
            listener.StartListeningForConnections(Port);

            var tcpClient = new TcpClient("localhost", Port);

            client = new PacketSocket(tcpClient);
            server = new PacketSocket(serverTask.Task.Result);

            Assert.IsTrue(client.IsConnected);
            Assert.IsTrue(server.IsConnected);
        }

        [TestMethod]
        public void ConnectParallel()
        {
            // error occured in endpoint manager tests:
            // TcpClient lost connection after beeing used to instantiate a PacketSocket

            const int PORT = 2312;

            const int COUNT = 20;

            TcpClientListener listener = new();
            listener.StartListeningForConnections(PORT);

            List<Task> tasks = new();
            List<TcpClient> clients = new();
            for(int i = 0; i < COUNT; i++)
            {
                var c = new TcpClient();
                clients.Add(c);
                tasks.Add(Task.Run(() => c.Connect("localhost", PORT)));
            }
            Task.WaitAll(tasks.ToArray());

            clients.ForEach(c =>
            {
                Assert.IsTrue(c.Connected);
                PacketSocket s = new PacketSocket(c);
                Assert.IsTrue(s.IsConnected);
                Assert.IsTrue(s.IsReceiving);

            });


        }

        [TestMethod]
        public void TestDisconnect()
        {
            const int PORT = 1234;
            using TcpClientListener listener = new();
            listener.StartListeningForConnections(PORT);

            List<PacketSocket> sockets = new();
            const int COUNT = 10;

            List<Task> tasks = new();
            for (int i = 0; i < COUNT; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    PacketSocket socket = new PacketSocket(new());
                    TestUtils.AssertTask(socket.ConnectAsync("localhost", PORT), 3000);
                    Assert.IsTrue(socket.IsConnected);

                    socket.DisconnectAsync().Wait();
                    Assert.IsFalse(socket.IsConnected);

                }));
            }
            TestUtils.AssertTask(Task.WhenAll(tasks), 3000);





            AsyncBarrier barrier = new(COUNT);

            ActionBlock<TcpClient> aB = new(c =>
            {
                c.Close();
                _=barrier.SignalAsync();
            });

            listener.ConnectingClientBlock.LinkTo(aB);



            for (int i = 0; i < COUNT; i++)
            {
                Task.Run(() =>
                {
                    PacketSocket socket = new PacketSocket(new());
                    TestUtils.AssertTask(socket.ConnectAsync("localhost", PORT), 3000);
                    Assert.IsTrue(socket.IsConnected);

                });
            }
            TestUtils.AssertTask(barrier.WaitAsync(), 3000);

        }

        [TestMethod]
        public void TestBasicFunctionality()
        {
            PacketSocket server, client;
            ResetSockets(out server, out client);

            TestPacketSocket_communication(server, client);


        }


        [TestMethod]
        public void TestDisconnectWhileSending()
        {
            PacketSocket server, client;
            ResetSockets(out server, out client);

            const int PAYLOAD_SIZE = 1024 * 100;

            Assert.IsTrue(server.IsConnected);
            Assert.IsTrue(client.IsConnected);

            byte[] payload = new byte[PAYLOAD_SIZE];

            for (int i = 0; i < payload.Length; i++)
            {
                payload[i] = (byte)i;
            }

            ActionBlock<byte[]> serverMessageBlock = new(buffer =>
            {
                Assert.IsFalse(true, "Message should not be received!");
            });

            using var serverDisp = server.IncomingBufferBlock.LinkTo(serverMessageBlock);


            var sendTask = client.SendAsync(payload);

            Assert.IsTrue(client.DisconnectAsync().Wait(100));
            Assert.IsFalse(client.IsConnected);

            Assert.IsTrue(sendTask.Wait(100), "sending timed out");

            //  Assert.IsFalse(sendTask.Result, "sending has not been aborted");


        }

        [TestMethod]
        public void TestDisconnectedSend()
        {
            TcpClient client = new();
            PacketSocket socket = new PacketSocket(client);

            byte[] payload = new byte[1024];

            TestUtils.AssertException<InvalidOperationException>(socket.SendAsync(payload));
        }

        [TestMethod]
        public void TestLocalRemoteDisconnect()
        {
            /*
             * ensure socket disconnect event is called properly
             * 
             * pre: server <-> client: connected
             * - disconnect client
             * -> server: remote disconnect
             * -> client: local disconnect
             */

            TaskCompletionSource tcs = new();

            const int PORT = 1634;
            using TcpClientListener listener = new();
            listener.StartListeningForConnections(PORT);
            PacketSocket server = null, client;
            ActionBlock<TcpClient> aB = new(c =>
            {
                server = new PacketSocket(c);
                tcs.SetResult();
            });
            listener.ConnectingClientBlock.LinkTo(aB);

            client = new PacketSocket();
            TestUtils.AssertTask(client.ConnectAsync("localhost", PORT), 3000);

            TestUtils.AssertTask(tcs.Task);

            Assert.IsTrue(server.IsConnected);
            Assert.IsTrue(client.IsConnected);
            Assert.IsTrue(server.IsReceiving);
            Assert.IsTrue(client.IsReceiving);

            TaskCompletionSource<bool> serverDisconnected = new();
            TaskCompletionSource<bool> clientDisconnected = new();

            bool onceServer = false;
            bool onceClient = false;

            EventHandler<DisconnectedArgs> serverDisconnect = (sender, e) =>
            {
                Assert.IsFalse(onceServer);
                onceServer = true;
                serverDisconnected.SetResult(e.RemoteDisconnect);
            };
            EventHandler<DisconnectedArgs> clientDisconnect = (sender, e) =>
            {
                Assert.IsFalse(onceClient);
                onceClient = true;
                clientDisconnected.SetResult(e.RemoteDisconnect);
            };

            client.Disconnected += clientDisconnect;
            server.Disconnected += serverDisconnect;

            TestUtils.AssertTask(client.DisconnectAsync());

            Assert.IsFalse(client.IsConnected);

            TestUtils.AssertTask(clientDisconnected.Task);
            TestUtils.AssertTask(serverDisconnected.Task);

            Assert.IsFalse(server.IsConnected);

            Assert.IsFalse(clientDisconnected.Task.Result, "socket falsely detected remote disconnect");
            Assert.IsTrue(serverDisconnected.Task.Result, "socket didn't detect remote disconnect");

            client.Disconnected -= clientDisconnect;
            server.Disconnected -= serverDisconnect;

        }

        private void TestPacketSocket_communication(PacketSocket server, PacketSocket client)
        {
            const int PAYLOAD_SIZE = 1024;
            const int COUNT = 100;

            Assert.IsTrue(server.IsConnected);
            Assert.IsTrue(client.IsConnected);

            byte[] payload = new byte[PAYLOAD_SIZE];

            for (int i = 0; i < payload.Length; i++)
            {
                payload[i] = (byte)i;
            }

            ActionBlock<byte[]> serverMessageBlock = new(buffer =>
            {
                CollectionAssert.AreEqual(payload, buffer);
            });
            ActionBlock<byte[]> clientMessageBlock = new(buffer =>
            {
                CollectionAssert.AreEqual(payload, buffer);
            });

            using var serverDisp = server.IncomingBufferBlock.LinkTo(serverMessageBlock);
            using var clientDisp = client.IncomingBufferBlock.LinkTo(clientMessageBlock);

            List<Task> taskList = new();
            for (int i = 0; i < COUNT; i++)
            {
                taskList.Add(Task.Run(async () =>
                {
                    Assert.IsTrue(await client.SendAsync(payload));
                }));
                taskList.Add(Task.Run(async () =>
                {
                    Assert.IsTrue(await server.SendAsync(payload));
                }));
            }

            Assert.IsTrue(Task.WhenAll(taskList).Wait(1000));
        }
    }
}
