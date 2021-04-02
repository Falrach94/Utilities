using MessageUtilities;
using MessageUtils.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MessageUtils
{
    public interface IMessageProcessor
    {
        BufferBlock<MessageProcessingError> ErrorBlock { get; }
        IDisposable RegisterMessageHandler(IMessageHandler handler);

        /// <summary>
        /// Waits for incoming response to given message id.
        /// </summary>
        /// <param name="responseId">>Id of original message.</param>
        /// <param name="timeout">time till exception is thrown</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">other handler already waits for this response id</exception>
        /// <exception cref="TimeoutException">wait timed out</exception>
        /// <exception cref="OperationCanceledException">operation was canceled</exception>
        Task<Message> WaitForResponseAsync(int responseId, TimeSpan timeout);
        Task<Message> WaitForResponseAsync(int responseId, TimeSpan timeout, CancellationToken token);
    }
}