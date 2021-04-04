using MessageUtility.MessageDictionary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTests.MessageUtils
{
    [TestClass]
    public class MessageDictionaryTests
    {
        const string Type = "TestType";
        const string Type2 = "TestType2";
        const string Module = "TestModule";

        const string TestValue = "Hello World!";

        [TestMethod]
        public void TestMessageDictionary()
        {
            var dic = MessageDictionary.GetInstance();

            //
            // register outgoing message
            //
            dic.AddOutgoingMessage(Module, Type, p =>
            {
                return Tuple.Create((string)p[0], (int)p[1]);
            }, typeof(Tuple<string, int>), "testString", "testInt");

            //
            // double register message
            //
            Assert.ThrowsException<ArgumentException>(() =>
            {
                dic.AddOutgoingMessage(Module, Type, p =>
                {
                    return Tuple.Create((string)p[0], (int)p[1]);
                }, typeof(Tuple<string, int>), "testString", "testInt");
            });

            //
            // create registered message
            //
            var msg = dic.CreateMessage(Type, TestValue, 42);

            Assert.IsTrue(msg.Module == Module);
            Assert.IsTrue(msg.Type == Type);
            Assert.IsTrue(msg.Data.GetType() == typeof(Tuple<string, int>));
            Assert.AreEqual(msg.Data, Tuple.Create(TestValue, 42));

            //
            // register and create message with complex type
            //
            dic.AddOutgoingMessage(Module, Type2, p =>
            {
                return Tuple.Create((string)p[0], (int)p[1]);
            }, "testString", "testInt");

            msg = dic.CreateMessage(Type2, TestValue, 42);

            Assert.IsTrue(msg.Module == Module);
            Assert.IsTrue(msg.Type == Type2);
            Assert.IsTrue(msg.Data.GetType() == typeof(Tuple<string, int>));
            Assert.AreEqual(msg.Data, Tuple.Create(TestValue, 42));

            //
            // create unregistered message
            //
            Assert.ThrowsException<ArgumentException>(() =>
            {
                dic.CreateMessage("abc");
            });

            //
            // register incoming message (type is already used for outgoing message)
            //
            dic.AddIngoingMessage(Module, Type, typeof(string), "testString");

            //
            // double register ingoing message
            //

            Assert.ThrowsException<ArgumentException>(() =>
            {
                dic.AddIngoingMessage(Module, Type, typeof(string), "testString");
            });

        }
    }
}
