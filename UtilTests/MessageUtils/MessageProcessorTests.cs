using MessageUtilities;
using MessageUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Endpoint;
using SyncUtils.Barrier;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using UtilTests.MessageUtils.Mocks;

namespace UtilTests.MessageUtils
{
    [TestClass]
    public class MessageProcessorTests
    {
        const string TestType = "test";
        const string TestType2 = "test2";

        private MessageFactory _msgFactory = new MessageFactory(TestType);
        private MessageFactory _msgFactory2 = new MessageFactory(TestType2);
        private MockSender _sender = new();

        private static RawMessage CreateRawMessage(MockSender sender, Message msg)
        {
            var task = msg.SerializeAsync();
            task.Wait();
            return new RawMessage(sender, task.Result);
        }
        private RawMessage CreateRawMessage(MockSender sender, string type, object data)
        {
            return CreateRawMessage(sender, _msgFactory.CreateMessage(type, data));
        }

        [TestMethod]
        public void TestMessageProcessor()
        {
            MockReceiver receiver = new();

            MessageProcessor processor = new();
            processor.SetMessageReceiver(receiver);

            TestSimpleMessageHandling(processor, receiver);

            TestMultiThreadMessageHandling(processor, receiver);

            TestWaitForResponse(processor, receiver);

            TestErrorForwarding(processor, receiver);
        }

        private void TestErrorForwarding(MessageProcessor processor, MockReceiver receiver)
        {
            const int COUNT = 10;
            AsyncBarrier barrier = new AsyncBarrier(COUNT);
            ActionBlock<MessageProcessingError> errorBlock = new(async error =>
            {
                Assert.AreEqual(_sender, error.Sender);
                await barrier.SignalAsync();
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = -1 });

            MockMessageHandler handler = new(TestType, (sender, msg) =>
            {
                throw new Exception("mock exception");
            });

            processor.ErrorBlock.LinkTo(errorBlock);

            for(int i = 0; i< COUNT; i++)
            {
                receiver.ReceiveMessage(CreateRawMessage(_sender, "test", null));
            }

            Assert.IsTrue(barrier.WaitAsync().Wait(100));
        }

        private void TestWaitForResponse(MessageProcessor processor, MockReceiver receiver)
        {
            const string Payload = "Hello World!";
            int id = 42;

            //
            // test if response is passed correctly
            //
            var task = processor.WaitForResponseAsync(id, Timeout.InfiniteTimeSpan);

            Message msg = _msgFactory.CreateMessage("test", Payload);
            msg.ResponseId = id;
            receiver.ReceiveMessage(CreateRawMessage(_sender, msg));

            Assert.IsTrue(task.Wait(100));

            var response = task.Result;

            Assert.AreEqual(response, msg);


            //
            // test if response of new message with same ID is passed correctly
            //
            task = processor.WaitForResponseAsync(id, Timeout.InfiniteTimeSpan);

            msg = _msgFactory.CreateMessage("tester", Payload);
            msg.ResponseId = id;
            receiver.ReceiveMessage(CreateRawMessage(_sender, msg));

            Assert.IsTrue(task.Wait(100));

            response = task.Result;

            Assert.AreEqual(response, msg);

            //
            // test if timeout is thrown
            //

            task = processor.WaitForResponseAsync(id, TimeSpan.FromMilliseconds(1));

            Assert.IsTrue(Assert.ThrowsExceptionAsync<TimeoutException>(async() =>
            {
                await task;
            }).Wait(1000));

            //
            // test if timeout is not thrown early and token cancelation works properly
            //
            for (int i = 0; i < 5; i++)
            {
                CancellationTokenSource cts = new();
                task = processor.WaitForResponseAsync(id, TimeSpan.FromMilliseconds(10000), cts.Token);

                Assert.IsFalse(task.Wait(100), "WaitForResponse ended early");

                cts.Cancel();
                Assert.IsTrue(Assert.ThrowsExceptionAsync<OperationCanceledException>(async() =>
                {
                    await task;
                }, "Operation canceled exception not thrown").Wait(100), "cancelation not within timeout");
            }

        }

        private void TestMultiThreadMessageHandling(MessageProcessor processor, MockReceiver receiver)
        {

            const int COUNT = 1000;

            List<Message> msgList = new();

            AsyncBarrier barrier = new(2 * COUNT + 1);

            SemaphoreSlim sem = new SemaphoreSlim(1,1);

            Func<object, Message, Task> handlerFct = async (sender, msg) =>
            {
                await sem.WaitAsync();
                Assert.IsTrue(msgList.Contains(msg));
                Assert.IsTrue(msgList.Remove(msg));
                sem.Release();

                await barrier.SignalAndWaitAsync();
            };

            MockMessageHandler handler = new(TestType, handlerFct);
            MockMessageHandler handler2 = new(TestType2, handlerFct);
            using var disp = processor.RegisterMessageHandler(handler);
            using var disp2 = processor.RegisterMessageHandler(handler2);

            AsyncBarrier syncBarrier = new(COUNT);

            List<Task> tasks = new();

            for (int i = 0; i < COUNT; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await sem.WaitAsync();
                    Message msg1 = _msgFactory.CreateMessage("test", "hello world");
                    msgList.Add(msg1);
                    Message msg2 = _msgFactory2.CreateMessage("test2", "hello world");
                    msgList.Add(msg2);
                    sem.Release();

                    await syncBarrier.SignalAndWaitAsync();

                    receiver.ReceiveMessage(CreateRawMessage(_sender, msg1));
                    receiver.ReceiveMessage(CreateRawMessage(_sender, msg2));

                }));
            }

            Assert.IsTrue(Task.WhenAll(tasks).Wait(100));

            Assert.IsTrue(barrier.SignalAndWaitAsync().Wait(100), $"list was not emptied before timeout (remaining {barrier.Remaining})");
            Assert.IsTrue(msgList.Count == 0);
        }

        private void TestSimpleMessageHandling(MessageProcessor processor,
                                               MockReceiver receiver)
        {
            TaskCompletionSource<Tuple<MockSender, Message>> tcs = new();

            MockMessageHandler handler = new(TestType, (sender, msg) =>
            {
                tcs.SetResult(Tuple.Create((MockSender)sender, msg));
                return Task.CompletedTask;
            });

            using (var disp = processor.RegisterMessageHandler(handler))
            {
                Assert.ThrowsException<ArgumentException>(() =>
                {
                    _ = processor.RegisterMessageHandler(handler);
                }, "multiple handler can be registered for same message type");
            }

            //ensure dispose removed handler 
            using (var disp = processor.RegisterMessageHandler(handler))
            {

                receiver.ReceiveMessage(CreateRawMessage(_sender, "test", null));

                Assert.IsTrue(tcs.Task.Wait(100), "message handler was not called");
            }
        }
    }
}
