using MessageUtilities;
using MessageUtils.Messenger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageUtils
{
    public interface IMultiTargetMessenger
    {
        /// <summary>
        /// Sends message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="SendFailedException">Sending message failed.</exception>
        Task SendMessageAsync(IMessageSender sender, Message msg);

        /// <summary>
        /// Sends message and waits for response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        /// <exception cref="SendFailedException">Sending message failed.</exception>
        /// <exception cref="UnexpectedResponseTypeException">actual response type didnt match expectation</exception>
        Task<Message> SendAndWaitForResponseAsync(IMessageSender sender, Message msg, int timeoutMs, params string[] responseTypes);

        Task RespondAsync(IMessageSender sender, Message original, Message response);
        Task RespondWithErrorAsync(IMessageSender sender, Message original, int errorType, string msg);
        Task RespondWithErrorAsync(IMessageSender sender, Message message, Enum error, string desc);
        Task RespondWithSuccessAsync(IMessageSender sender, Message original, object result);
        Task RespondWithSuccessAsync(IMessageSender sender, Message original);
    }
}