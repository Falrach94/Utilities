using MessageUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtils.Messenger.Interfaces
{
    public interface ISingleTargetMessenger
    {
        void SetTarget(IMessageSender receiver);

        Task SendMessageAsync(Message msg);

        Task<Message> SendAndWaitForResponseAsync(Message msg, int timeoutMs, params string[] responseTypes);

        Task RespondAsync(Message original, Message response);
        Task RespondWithErrorAsync(Message original, int errorType, string msg);
        Task RespondWithErrorAsync(Message message, Enum error, string desc);
        Task RespondWithSuccessAsync(Message original, object result);
        Task RespondWithSuccessAsync(Message original);
    }
}
