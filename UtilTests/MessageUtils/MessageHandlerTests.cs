using MessageUtilities;
using MessageUtility.MessageDictionary;
using MessageUtils;
using MessageUtils.MessageHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Endpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilTests.MessageUtils.Mocks;

namespace UtilTests.MessageUtils
{
    public class TestMessageHandler : MessageHandler
    {
        public TaskCompletionSource Tcs = new();

        public TestMessageHandler() : base("TestModule")
        {
        }

        protected override void MessageRegistration()
        {
            RegisterMessage("TesterType", Handler, typeof(Tuple<string, int>), "testString", "testInt");

            Assert.ThrowsException<ArgumentException>(() =>
            {
                RegisterMessage("TesterType", Handler, typeof(Tuple<string, int>), "testString", "testInt");
            });
        }

        private Task Handler(object sender, Message message)
        {
            Assert.IsTrue(((MockSender)sender).Id == 42);
            message.GetData<Tuple<string, int>>().Equals(Tuple.Create("Hello World!", 42));
            Tcs.SetResult();
            return Task.CompletedTask;
        }
    }

    [TestClass]
    public class MessageHandlerTests
    {
        const string Module = "TestModule";
        const string Type = "TesterType";
        const string TestValue = "Hello World!";

        [TestMethod]
        public void TestMessageHandler()
        {
            MockSender sender = new() { Id = 42};

            TestMessageHandler handler = new();

            MockReceiver receiver = new();

            MessageProcessor processor = new();
            processor.SetMessageReceiver(receiver);

            MessageDictionary dic = MessageDictionary.GetInstance();

            processor.RegisterMessageHandler(handler);

            dic.AddOutgoingMessage(Module, Type, p =>
            {
                return Tuple.Create(TestValue, 42);
            });

            var msg = dic.CreateMessage(Type);

            receiver.ReceiveMessage(new RawMessage(sender, msg.Serialize()));

            Assert.IsTrue(handler.Tcs.Task.Wait(100));



        }

    }
}
