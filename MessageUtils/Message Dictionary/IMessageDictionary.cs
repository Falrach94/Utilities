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
        /// <summary>
        /// Registers outgoing message for pdf creation. 
        /// Registered messages can be instantiated by a call to CreateMessage.
        /// </summary>
        /// <param name="module">message module</param>
        /// <param name="type">message type</param>
        /// <param name="dataFactory">Factory function for this message, which will be called by CreateMessage.</param>
        /// <param name="dataType">message data type</param>
        /// <param name="desc">description for each data field</param>
        /// <exception cref="ArgumentException">message with this type is already registered</exception>
        void AddOutgoingMessage(string module, string type, Func<object[], object> dataFactory, Type dataType, params string[] desc);
        void AddOutgoingMessage<T>(string module, string type, Func<object[], T> dataFactory,  params string[] desc);

        /// <summary>
        /// Registers ingoing message for pdf creation.
        /// </summary>
        /// <param name="module">message module</param>
        /// <param name="type">message type</param>
        /// <param name="dataType">type of message data</param>
        /// <param name="desc">description for each data field</param>
        void AddIngoingMessage(string module, string type, Type dataType, params string[] desc);

        /// <summary>
        /// Creates message of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">no registered outgoing message with this type found<exception>
        Message CreateMessage(string type, params object[] data);

        void CreatePDF(string path);
    }
}
