using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Socket;
using NetworkUtils.Tcp_Client_Listener;
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
        public void TestBasicFunctionality()
        {
            PacketSocket server, client;
            ResetSockets(out server, out client);

            TestPacketSocket_communication(server, client);
            TestPacketSocket_disconnect(server, client);
            TestPacketSocket_disconnectedSend(server, client);

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

        private void TestPacketSocket_disconnectedSend(PacketSocket server, PacketSocket client)
        {
            Assert.IsFalse(server.IsConnected);
            Assert.IsFalse(client.IsConnected);

            byte[] payload = new byte[1024];

            TestUtils.AssertException<InvalidOperationException>(server.SendAsync(payload));
        }

        private void TestPacketSocket_disconnect(PacketSocket server, PacketSocket client)
        {
            const int Timeout = 100;

            Assert.IsTrue(server.IsConnected);
            Assert.IsTrue(client.IsConnected);

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

            client.DisconnectAsync().Wait();

            clientDisconnected.Task.Wait(Timeout);
            serverDisconnected.Task.Wait(Timeout);

            Assert.IsFalse(client.IsConnected);
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
