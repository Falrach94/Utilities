using MessageUtilities;
using MessageUtils;
using MessageUtils.Messenger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Endpoint;
using SyncUtils;
using SyncUtils.Barrier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilTests.MessageUtils.Mocks;

namespace UtilTests.MessageUtils
{
    [TestClass]
    public class MessengerTests
    {
        
        [TestMethod]
        public void TestSingleTargetMessenger()
        {
            const string Module = "testModule";
            const string Type = "testType";

            MessageFactory msgFactory = new(Module);

            int expectedValue = 42;
            int expectedId = 0;
            int expectedResponseId = -1;

            TaskCompletionSource tcs = new();

            MockReceiver receiver = new();
            MessageProcessor processor = new();
            processor.SetMessageReceiver(receiver);
            Action<MockSender, Message> sendHandler = (sender, msg) =>
            {
                Assert.AreEqual(msg.Module, Module);
                Assert.AreEqual(msg.Type, Type);
                Assert.AreEqual(msg.Id, expectedId);
                Assert.AreEqual(msg.ResponseId, expectedResponseId);
                Assert.AreEqual((int)msg.Data, expectedValue);

                if (msg.IsResponse)
                {
                    receiver.ReceiveMessage(new RawMessage(sender, msg.Serialize()));
                }

                tcs.TrySetResult();
            };

            MockSender sender = new(sendHandler);

            SingleTargetMessenger messenger = new(processor, sender);


            //
            // SendMessageAsync
            //

            var testMessage = msgFactory.CreateMessage(Type, expectedValue);

            Assert.IsTrue(messenger.SendMessageAsync(testMessage).Wait(100));

            Assert.IsTrue(tcs.Task.Wait(100));

            expectedId++;

            //
            // SendMessageAsync failed
            //

            sender.Working = false;

            Assert.IsTrue(Assert.ThrowsExceptionAsync<SendFailedException>(async() =>
            {
                await messenger.SendMessageAsync(testMessage);
            }).Wait(100));

            expectedId++;
            sender.Working = true;

            //
            // SendAndWaitForResponseAsync, RespondAsync
            //

            Assert.IsTrue(Assert.ThrowsExceptionAsync<TimeoutException>(async () =>
            {
                await messenger.SendAndWaitForResponseAsync(testMessage, 10);
            }).Wait(100), "timeout didn't occour in time");

            expectedId++;

            tcs = new();

            var task = messenger.SendAndWaitForResponseAsync(testMessage, 100);

            Assert.IsTrue(tcs.Task.Wait(100));
            expectedResponseId = expectedId;
            expectedId++;
            var response = msgFactory.CreateMessage(Type, expectedValue);
            messenger.RespondAsync(testMessage, response);

            Assert.IsTrue(task.Wait(100), "response was not recognized in time");

        }
        
        
        [TestMethod]
        public void TestBroadcastMessenger_BroadcastAndWait_SendingException()
        {
            const int Count = 20;
            List<MockSender> sender = new();

            const string Module = "testModule";
            const string Type = "testType";

            MessageFactory msgFactory = new(Module);


            MockReceiver receiver = new();
            MessageProcessor processor = new();
            processor.SetMessageReceiver(receiver);

            BroadcastMessenger messenger = new(processor);


            for (int i = 0; i < Count; i++)
            {
                sender.Add(new MockSender());
                sender.Last().Working = false;
            }



            var msg = msgFactory.CreateMessage(Type, null);

            var task = messenger.BroadcastMessageAndWaitAsync(sender, msg, 10);

            Assert.IsTrue(task.Wait(100), "broadcast did not finish in time");

            var responses = task.Result;
            Assert.IsTrue(responses.Count == Count);
            for (int i = 0; i < responses.Count; i++)
            {
                Assert.IsTrue(responses[i] == null);
            }
        }


        [TestMethod]
        public void TestBroadcastMessenger_BroadcastAndWait_Timeout()
        {
            const int Count = 20;
            List<MockSender> sender = new();

            const string Module = "testModule";
            const string Type = "testType";

            MessageFactory msgFactory = new(Module);

            AsyncBarrier barrier = new(Count);

            MockReceiver receiver = new();
            MessageProcessor processor = new();
            processor.SetMessageReceiver(receiver);

            BroadcastMessenger messenger = new(processor);


            for (int i = 0; i < Count; i++)
            {
                sender.Add(new MockSender());
                sender.Last().Working = true;
            }
            var msg = msgFactory.CreateMessage(Type, null);

            var task = messenger.BroadcastMessageAndWaitAsync(sender, msg, 10);

            Assert.IsTrue(task.Wait(100), "broadcast did not finish in time");

            var responses = task.Result;
            Assert.IsTrue(responses.Count == Count);
            for (int i = 0; i < responses.Count; i++)
            {
                Assert.IsTrue(responses[i] == null);
            }

        }

        [TestMethod]
        public void TestBroadcastMessenger_BroadcastAndWait()
        {
            const int Count = 20;
            List<MockSender> sender = new();

            const string Module = "testModule";
            const string Type = "testType";

            MessageFactory msgFactory = new(Module);

            AsyncBarrier barrier = new(Count);

            MockReceiver receiver = new();
            MessageProcessor processor = new();
            processor.SetMessageReceiver(receiver);

            BroadcastMessenger messenger = new(processor);


            Action<MockSender, Message> sendFnc = (sender, msg) =>
            {
                var response = msgFactory.CreateMessage("response", null);
                response.ResponseId = msg.Id;
                receiver.ReceiveMessage(new RawMessage(sender, response.Serialize()));

            };

            for (int i = 0; i < Count; i++)
            {
                sender.Add(new MockSender(sendFnc));
                sender.Last().Working = true;
            }
            var msg = msgFactory.CreateMessage(Type, null);

            var task = messenger.BroadcastMessageAndWaitAsync(sender, msg, 1000);

            Assert.IsTrue(task.Wait(100), "broadcast did not finish in time");

            var responses = task.Result;
            Assert.IsTrue(responses.Count == Count);
            for (int i = 0; i < responses.Count; i++)
            {
                Assert.IsTrue(responses[i] != null, "response is null");
            }

        }
        [TestMethod]
        public void TestBroadcastMessenger_BroadcastAndWait_Mixed()
        {
            const int Count = 10;
            List<MockSender> sender = new();

            const string Module = "testModule";
            const string Type = "testType";

            MessageFactory msgFactory = new(Module);

            SemaphoreLock semLock = new();
            HashSet<int> ids = new();

            AsyncBarrier barrier = new(Count);

            MockReceiver receiver = new();
            MessageProcessor processor = new();
            processor.SetMessageReceiver(receiver);

            BroadcastMessenger messenger = new(processor);

            Action<MockSender, Message> sendFnc = (sender, msg) =>
            {
                var task = semLock.LockAsync();
                task.Wait();
                using var token = task.Result;

                Assert.IsFalse(ids.Contains(msg.Id), $"message id {msg.Id} was used multiple times while broadcasting");
                ids.Add(msg.Id);
                barrier.SignalAsync().Wait();
                Assert.IsTrue(barrier.Remaining >= 0);

                if (msg.Id %2 == 0)
                {
                    var response = msgFactory.CreateMessage("response", null);
                    response.ResponseId = msg.Id;

                    receiver.ReceiveMessage(new RawMessage(sender, response.Serialize()));
                }
            };

            for (int i = 0; i < Count; i++)
            {
                sender.Add(new MockSender(sendFnc));
                sender.Last().Working = false;
            }
            for (int i = 0; i < Count; i++)
            {
                sender.Add(new MockSender(sendFnc));
                sender.Last().Working = true;
            }



            var msg = msgFactory.CreateMessage(Type, null);

            var task = messenger.BroadcastMessageAndWaitAsync(sender, msg, 100);

            Assert.IsTrue(task.Wait(2000), "broadcast did not finish in time");
            Assert.IsTrue(barrier.WaitAsync().Wait(100), "broadcast messages were not sent");
            var responses = task.Result;
            Assert.IsTrue(responses.Count == sender.Count);
            for(int i = 0; i < responses.Count; i++)
            {
                if(i < Count)
                {
                    Assert.IsTrue(responses[i] == null);
                }
                else
                {
                    if(responses[i] != null)
                    {
                        Assert.IsTrue(responses[i].Id % 2 == 0);
                    }
                }
            }

        }

        [TestMethod]
        public void TestBroadcastMessenger_BroadcastMessage()
        {
            const int Count = 10;
            List<MockSender> sender = new();

            const string Module = "testModule";
            const string Type = "testType";

            MessageFactory msgFactory = new(Module);

            SemaphoreLock semLock = new();
            HashSet<int> ids = new();

            AsyncBarrier barrier = new(Count);

            Action<MockSender, Message> sendFnc = (sender, msg) =>
            {
                var task = semLock.LockAsync();
                task.Wait();
                using var token = task.Result;

                Assert.IsFalse(ids.Contains(msg.Id), $"message id {msg.Id} was used multiple times while broadcasting");
                ids.Add(msg.Id);
                barrier.SignalAsync().Wait();
                Assert.IsTrue(barrier.Remaining >= 0);

            };

            for (int i = 0; i < Count; i++)
            {
                sender.Add(new MockSender(sendFnc));
                sender.Last().Working = false;
            }
            for (int i = 0; i < Count; i++)
            {
                sender.Add(new MockSender(sendFnc));
                sender.Last().Working = true;
            }


            MockReceiver receiver = new();
            MessageProcessor processor = new();
            processor.SetMessageReceiver(receiver);

            BroadcastMessenger messenger = new(processor);

            var msg = msgFactory.CreateMessage(Type, null);

            Assert.IsTrue(messenger.BroadcastMessageAsync(sender, msg).Wait(500));
            Assert.IsTrue(barrier.WaitAsync().Wait(100));
        }
    }
}
