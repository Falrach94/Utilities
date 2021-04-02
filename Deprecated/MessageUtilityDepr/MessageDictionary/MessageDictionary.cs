using MessageUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtility.MessageDictionary
{
    public class MessageDictionary : IMessageDictionary
    {
        private readonly Dictionary<string, ISet<MessageInfo>> _incomingMessageDic = new Dictionary<string, ISet<MessageInfo>>();
        private readonly Dictionary<string, MessageInfo> _outgoingMessageDic = new Dictionary<string, MessageInfo>();
        private readonly ISet<string> _categories = new HashSet<string>();

        private MessageDictionary()
        {
        }

        private static MessageDictionary _instance;

        public static MessageDictionary GetInstance()
        {
            if(_instance == null)
            {
                _instance = new MessageDictionary();
            }
            return _instance;
        }

        private void PrintMessageInfo(MessageInfo info, StreamWriter writer)
        {
            writer.WriteLine("\tmessage: " + info.Type);
            if (info.DataType != null)
            {
                writer.WriteLine("\ttype: " + info.DataType);
            }
            int i = 1;
            foreach(var p in info.ParamDescriptions)
            {
                writer.WriteLine("\t (param " + i++ +") " + p);                
            }
        }

        public void CreatePDF(string path)
        {

            using(var stream = new FileStream(path, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    foreach(var c in _categories)
                    {
                        writer.WriteLine("Module: " + c + "\n");
                        writer.WriteLine("   Outgoing:\n");

                        var messages = _outgoingMessageDic.Values.Where(info => info.Module == c);

                        foreach(var m in messages)
                        {
                            PrintMessageInfo(m, writer);
                            writer.WriteLine("\n");
                        }
                        writer.WriteLine("   Incoming:\n");

                        foreach (var m in _incomingMessageDic[c])
                        {
                            PrintMessageInfo(m, writer);
                            writer.WriteLine("\n");
                        }
                        writer.WriteLine("\n");
                    }

                }
            }
        }
        public void AddOutgoingMessage(string module, string type)
        {
            AddOutgoingMessage(module, type, null, null);
        }
        public void AddOutgoingMessage(string module, string type, Func<object[], object> factory, Type dataType, params string[] desc)
        {
            if (!_categories.Contains(module))
            {
                _categories.Add(module);
                _incomingMessageDic.Add(module, new HashSet<MessageInfo>());
            }
            var info = new MessageInfo(module, type, factory, dataType, desc);
            _outgoingMessageDic.Add(type, info);
        }
        public void AddIngoingMessage(string module, string type, Type dataType, params string[] desc)
        {
            if (!_categories.Contains(module))
            {
                _categories.Add(module);
                _incomingMessageDic.Add(module, new HashSet<MessageInfo>());
            }

            var info = new MessageInfo(module, type, dataType, desc);
            _incomingMessageDic[module].Add(info);
        }

        public Message CreateMessage(string type, params object[] data)
        {
            if(!_outgoingMessageDic.ContainsKey(type))
            {
                throw new ArgumentException("Message of type '" +type+"' not found in message dictionary!");
            }

            var msg = _outgoingMessageDic[type];
            var st = new SerializableStorage();
            if (msg.Factory != null)
            {
                st.Add("Data", msg.Factory(data));
            }
            return new Message(msg.Module, msg.Type, st);
        }

    }
}
