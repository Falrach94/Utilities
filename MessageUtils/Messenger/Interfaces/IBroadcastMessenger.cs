using MessageUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtils.Messenger
{
    public interface IBroadcastMessenger
    {
        /// <summary>
        /// Sends a copy of the specified message to a collection of receivers.
        /// Failed sending attempts are ignored.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task BroadcastMessageAsync(IEnumerable<IMessageSender> receiver, Message msg);

        /// <summary>
        /// Sends a copy of the specified message to a collection of receivers and waits for their responses.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="msg"></param>
        /// <param name="timeoutMs"></param>
        /// <returns>List with response messages. If a sending failed or the response timed out, the corresponding list entry will be null.</returns>
        Task<List<Message>> BroadcastMessageAndWaitAsync(IEnumerable<IMessageSender> receiver, Message msg, int timeoutMs);
    }
}
