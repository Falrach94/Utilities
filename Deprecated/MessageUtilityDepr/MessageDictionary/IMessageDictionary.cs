using MessageUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtility.MessageDictionary
{
    public interface IMessageDictionary
    {
        void AddOutgoingMessage(string module, string type, Func<object[], object> factory, Type dataType, params string[] desc);
        void AddIngoingMessage(string module, string type, Type dataType, params string[] desc);

        Message CreateMessage(string type, params object[] data);

        void CreatePDF(string path);
    }
}
