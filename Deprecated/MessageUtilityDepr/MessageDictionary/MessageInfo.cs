using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtility.MessageDictionary
{
    public class MessageInfo
    {
        public string Module { get; }
        public string Type { get; }
        public Func<object[], object> Factory { get; }
        public Type DataType { get; }
        public string[] ParamDescriptions { get; }

        public MessageInfo(string module, string type, Func<object[], object> factory, Type dataType, string[] desc)
        {
            Module = module;
            Type = type;
            Factory = factory;
            DataType = dataType;
            ParamDescriptions = desc;
        }

        public MessageInfo(string module, string type, Type dataType, string[] desc)
        {
            Module = module;
            Type = type;
            DataType = dataType;
            ParamDescriptions = desc;
        }
    }
}
