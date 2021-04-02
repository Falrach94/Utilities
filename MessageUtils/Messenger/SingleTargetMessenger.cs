using MessageUtilities;
using MessageUtils.Messenger.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtils.Messenger
{
    public class SingleTargetMessenger : ISingleTargetMessenger
    {
        private IMessageSender _target;

        private readonly MultiTargetMessenger _messenger;

        public SingleTargetMessenger(IMessageProcessor messageProcessor,
                                     IMessageSender target)
        {
            _messenger = new(messageProcessor);
            SetTarget(target);
        }

        public Task RespondAsync(Message original, Message response)
        {
            return _messenger.RespondAsync(_target, original, response);
        }

        public Task RespondWithErrorAsync(Message original, int errorType, string msg)
        {
            return _messenger.RespondWithErrorAsync(_target, original, errorType, msg);
        }

        public Task RespondWithErrorAsync(Message message, Enum error, string desc)
        {
            return _messenger.RespondWithErrorAsync(_target, message, error, desc);
        }

        public Task RespondWithSuccessAsync(Message original, object result)
        {
            return _messenger.RespondWithSuccessAsync(_target, original, result);
        }

        public Task RespondWithSuccessAsync(Message original)
        {
            return _messenger.RespondWithSuccessAsync(_target, original);
        }

        public Task<Message> SendAndWaitForResponseAsync(Message msg, int timeoutMs, params string[] responseTypes)
        {
            return _messenger.SendAndWaitForResponseAsync(_target, msg, timeoutMs, responseTypes);
        }

        public Task SendMessageAsync(Message msg)
        {
            return _messenger.SendMessageAsync(_target, msg);
        }

        public void SetTarget(IMessageSender target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }
    }
}
