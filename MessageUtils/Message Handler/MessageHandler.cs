using MessageUtilities;
using MessageUtility.MessageDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtils.MessageHandler
{
    public delegate Task MessageHandlerDelegate(object sender, Message message);
    public abstract class MessageHandler : IMessageHandler
    {
        private static readonly MessageDictionary Messages = MessageDictionary.GetInstance();

        public string Module { get; }


        private readonly Dictionary<string, MessageHandlerDelegate> _handlerDic = new();


        protected MessageHandler(string module)
        {
            Module = module;
            MessageRegistration();
        }

        /// <summary>
        /// Registeres message handler for the specified message type and adds ingoing message informations to MessageDictionary.
        /// </summary>
        /// <param name="type">message type</param>
        /// <param name="handler">message handler</param>
        /// <param name="dataType">type of message data for message dictionary (may be null)</param>
        /// <param name="desc">description of message data fields for message dictionary</param>
        /// <exception cref="ArgumentException">type is invalid or already in use</exception>
        protected void RegisterMessage(string type,
                                       MessageHandlerDelegate handler,
                                       Type dataType,
                                       params string[] desc)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException($"{nameof(type)} darf nicht NULL oder leer sein.", nameof(type));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (desc is null)
            {
                throw new ArgumentNullException(nameof(desc));
            }

            if (_handlerDic.ContainsKey(type))
            {
                throw new ArgumentException("Handler for this type was already registered!");
            }
            _handlerDic.Add(type, handler);
            Messages.AddIngoingMessage(Module, type, dataType, desc);
        }

        protected abstract void MessageRegistration();

        public Task HandleMessage(object sender, Message msg)
        {
            var type = msg.Type;

            if (!_handlerDic.ContainsKey(type))
            {
                throw new UnknownMessageTypeException();
            }
            else
            {
                return _handlerDic[type](sender, msg);
            }
        }
    }
}
