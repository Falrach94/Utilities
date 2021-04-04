using MessageUtilities;
using MessageUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Endpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTests.MessageUtils
{
    [TestClass]
    public class MessageSerializationTests
    {
        const string Module = "TestModule";
        const string Type = "TestType";
        const string Data = "Hello World!";
        enum TestEnum
        {
            TestType
        }
        [TestMethod]
        public void TestMessageFactory()
        {
            MessageFactory factory = new(Module);

            var msg = factory.CreateMessage(Type, Data);

            Assert.IsTrue(msg.Module.Equals(Module));
            Assert.IsTrue(msg.Type.Equals(Type));
            Assert.IsTrue(msg.Data.Equals(Data));

            msg = factory.CreateMessage(TestEnum.TestType, Data);

            Assert.IsTrue(msg.Module.Equals(Module));
            Assert.IsTrue(msg.Type.Equals(Type));
            Assert.IsTrue(msg.Data.Equals(Data));

        }
        [TestMethod]
        public void TestMessageSerialization()
        {
            MessageFactory factory = new(Module);
            var msg = factory.CreateMessage(Type, Data);

            {
                var data = msg.Serialize();
                var task = Message.DeserializeAsync(data);
                Assert.IsTrue(task.Wait(100));
                var newMessage = task.Result;

                Assert.IsTrue(msg.Module.Equals(newMessage.Module), "Module not correct");
                Assert.IsTrue(msg.Type.Equals(newMessage.Type), "Type not correct");
                Assert.IsTrue(msg.Data.Equals(newMessage.Data), "Data not correct");
                Assert.IsTrue(msg.Id.Equals(newMessage.Id), "Id not correct");
                Assert.IsTrue(msg.ResponseId.Equals(newMessage.ResponseId), "ResponseId not correct");
            }
            {
                msg.Data = null;

                var data = msg.Serialize();
                var task = Message.DeserializeAsync(data);
                Assert.IsTrue(task.Wait(100));
                var newMessage = task.Result;

                Assert.IsTrue(msg.Module.Equals(newMessage.Module), "Module not correct");
                Assert.IsTrue(msg.Type.Equals(newMessage.Type), "Type not correct");
                Assert.IsTrue(msg.Data == newMessage.Data, "Data not correct");
                Assert.IsTrue(msg.Id.Equals(newMessage.Id), "Id not correct");
                Assert.IsTrue(msg.ResponseId.Equals(newMessage.ResponseId), "ResponseId not correct");
            }

            {
                msg.Data = new Tuple<string, int>(Data, 42);

                var data = msg.Serialize();
                var task = Message.DeserializeAsync(data);
                Assert.IsTrue(task.Wait(100));
                var newMessage = task.Result;

                Assert.IsTrue(msg.Module.Equals(newMessage.Module), "Module not correct");
                Assert.IsTrue(msg.Type.Equals(newMessage.Type), "Type not correct");
                Assert.IsTrue(msg.Data.Equals(newMessage.Data), "Data not correct");
                Assert.IsTrue(msg.Id.Equals(newMessage.Id), "Id not correct");
                Assert.IsTrue(msg.ResponseId.Equals(newMessage.ResponseId), "ResponseId not correct");
            }

            {
                var taskSer = msg.SerializeAsync();
                Assert.IsTrue(taskSer.Wait(100));
                var data = taskSer.Result;

                var task = Message.DeserializeAsync(data);
                Assert.IsTrue(task.Wait(100));
                var newMessage = task.Result;

                Assert.IsTrue(msg.Module.Equals(newMessage.Module));
                Assert.IsTrue(msg.Type.Equals(newMessage.Type));
                Assert.IsTrue(msg.Data.Equals(newMessage.Data));
                Assert.IsTrue(msg.Id.Equals(newMessage.Id));
                Assert.IsTrue(msg.ResponseId.Equals(newMessage.ResponseId));
            }

            {
                var data = msg.Serialize();

                var rawMessage = new RawMessage(null, data);

                var task = Message.FromRawMessageAsync(rawMessage);
                Assert.IsTrue(task.Wait(100));
                var newMessage = task.Result;

                Assert.IsTrue(msg.Module.Equals(newMessage.Module));
                Assert.IsTrue(msg.Type.Equals(newMessage.Type));
                Assert.IsTrue(msg.Data.Equals(newMessage.Data));
                Assert.IsTrue(msg.Id.Equals(newMessage.Id));
                Assert.IsTrue(msg.ResponseId.Equals(newMessage.ResponseId));
            }




        }
    }
}
