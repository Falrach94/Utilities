using MessageUtilities;
using PatternUtils.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtils.Messenger
{
    public class MultiTargetMessenger : IMultiTargetMessenger
    {
        private readonly IdPool _idPool = new();
        private readonly IMessageProcessor _messageProcessor;

        public MultiTargetMessenger(IMessageProcessor messageProcessor)
        {
            _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
        }

        public Task RespondAsync(IMessageSender sender, Message original, Message response)
        {
            response.ResponseId = original.Id;
            return SendMessageAsync(sender, response);
        }

        public Task RespondWithErrorAsync(IMessageSender sender, Message original, int errorType, string msg)
        {
            Message errorMsg = new Message(original.Module, "error", new Error(errorType, msg));
            return RespondAsync(sender, original, errorMsg);
        }

        public Task RespondWithErrorAsync(IMessageSender sender, Message message, Enum errorType, string msg)
        {
            return RespondWithErrorAsync(sender, message, (int)(object)errorType, msg);
        }

        public Task RespondWithSuccessAsync(IMessageSender sender, Message original, object result)
        {
            Message msg = new Message(original.Module, "success", result);
            return RespondAsync(sender, original, msg);
        }

        public Task RespondWithSuccessAsync(IMessageSender sender, Message original)
        {
            return RespondWithSuccessAsync(sender, original, null);
        }

        public async Task<Message> SendAndWaitForResponseAsync(IMessageSender sender, Message msg, int timeoutMs, params string[] responseTypes)
        {
            _idPool.GetNextId(out int id);
            var responseTask = _messageProcessor.WaitForResponseAsync(id, TimeSpan.FromMilliseconds(timeoutMs));
            await SendMessageAsync(sender, msg, id);
            var response = await responseTask;

            if(responseTypes.Length != 0 && !responseTypes.Contains(response.Type))
            {
                throw new UnexpectedResponseTypeException();
            }
            return response;
        }

        private async static Task SendMessageAsync(IMessageSender sender, Message msg, int id)
        {
            msg.Id = id;
            if(!await sender.SendAsync(msg))
            {
                throw new SendFailedException();
            }
        }

        public Task SendMessageAsync(IMessageSender sender, Message msg)
        {
            _idPool.GetNextId(out int id);
            return SendMessageAsync(sender, msg, id);
        }
    }
}
