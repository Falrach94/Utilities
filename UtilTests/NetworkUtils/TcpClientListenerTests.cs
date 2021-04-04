using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Socket;
using NetworkUtils.Tcp_Client_Listener;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using UtilTests.TestUtilities;

namespace UtilTests.NetworkUtils
{
    [TestClass]
    public class TcpClientListenerTests
    {
        [TestMethod]
        public void StartStop()
        {
            TcpClientListener listener = new();
            const int Port = 1234;

            listener.StartListeningForConnections(Port);

            Assert.AreEqual(listener.Port, Port);

            Assert.IsTrue(listener.IsListening);

            TestUtils.AssertTask(listener.StopListeningForConnectionsAsync());

            Assert.IsFalse(listener.IsListening);

            listener.StartListeningForConnections(Port);

            Assert.IsTrue(listener.IsListening);

            TestUtils.AssertTask(listener.StopListeningForConnectionsAsync());

            Assert.IsFalse(listener.IsListening);

            //connect to stopped listener
            TcpClient client = new();
            TestUtils.AssertTaskTimeout(client.ConnectAsync("localhost", Port));

        }
        
        [TestMethod]
        public void StartOnNewPort()
        {
            const int PortOld = 4325;
            const int PortNew = 5234;

            TcpClientListener listener = new();
            listener.StartListeningForConnections(PortOld);
            Assert.IsTrue(listener.IsListening);
            Assert.AreEqual(PortOld, listener.Port);

            TestUtils.AssertTask(listener.StopListeningForConnectionsAsync());

            Assert.IsFalse(listener.IsListening);
            Assert.AreEqual(PortOld, listener.Port);

            listener.StartListeningForConnections(PortNew);
            Assert.AreEqual(listener.Port, PortNew);
            Assert.IsTrue(listener.IsListening);
        }
        [TestMethod]
        public void AcceptConnections()
        {
            const string TestText = "Hello World!";
            const int Port = 4321;
            TcpClientListener listener = new();
            listener.StartListeningForConnections(Port);

            ActionBlock<TcpClient> acceptClient = new(async client =>
            {
                using StreamWriter writer = new(client.GetStream());
                writer.WriteLine(TestText);
                await Task.Delay(100);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = -1 });

            using var _ = listener.ConnectingClientBlock.LinkTo(acceptClient);

            Assert.IsTrue(listener.IsListening);

            const int COUNT = 10;

            List<TcpClient> clientList = new();

            List<Task> taskList = new();

            for (int i = 0; i < COUNT; i++)
            {
                taskList.Add(Task.Run(async () =>
                {
                    var cts = new CancellationTokenSource(3000);
                    TcpClient client = new();

                    await client.ConnectAsync("localhost", Port, cts.Token);

                    clientList.Add(client);
                    using StreamReader reader = new(client.GetStream());
                    Assert.AreEqual(reader.ReadLine(), TestText);
                }));
            }
            Task.WhenAll(taskList).Wait();

            listener.StopListeningForConnectionsAsync().Wait();
        }
        [TestMethod]
        public void DisposeListener()
        {
            const int Port = 4321;
            TcpClientListener listenerDisp;
            using (listenerDisp = new TcpClientListener())
            {
                listenerDisp.StartListeningForConnections(Port);
            }
            Assert.IsFalse(listenerDisp.IsListening);
        }
        [TestMethod]
        public void DoubleStart()
        {
            TcpClientListener listener = new();
            const int Port = 4321;
            Assert.IsFalse(listener.IsListening);
            listener.StartListeningForConnections(Port);
            Assert.IsTrue(listener.IsListening);
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                listener.StartListeningForConnections(Port);
            });
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                listener.StartListeningForConnections(Port+1);
            });

        }



 

    }
}
