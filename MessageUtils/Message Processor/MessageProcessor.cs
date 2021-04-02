
using MessageUtilities;
using MessageUtils.MessageHandler;
using NetworkUtils.Endpoint;
using PatternUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MessageUtils
{
    /// <summary>
    /// Consumes raw messages, parses them and distributes them either to a message handler in case of a normal message, or to the waiting response handler.
    /// Occouring errors and exceptions while handling a message are passed to the ErrorBlock.
    /// </summary>
    public class MessageProcessor : IMessageProcessor, IUnsubscribeable<IMessageHandler>, IDisposable
    {
        private readonly ActionBlock<RawMessage> _messageHandlerBlock;
        private readonly Dictionary<string, IMessageHandler> _messageHandlerDic = new();

        private readonly Dictionary<int, Tuple<Timer, TaskCompletionSource<Message>>> _responseDic = new();
        private readonly SemaphoreSlim _sem = new(1, 1);

        private IDisposable _unsubscriber;

        public BufferBlock<MessageProcessingError> ErrorBlock { get; } = new();


        public MessageProcessor()
        {
            _messageHandlerBlock = new(HandleMessage, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = -1 });

        }

        public void SetMessageReceiver(IRawMessageReceiver receiver)
        {
            if (_unsubscriber is not null)
            {
                _unsubscriber.Dispose();
                _unsubscriber = null;
            }

            if (receiver != null)
            {
                _unsubscriber = receiver.RawMessageBlock.LinkTo(_messageHandlerBlock);
            }
        }

        private async Task HandleMessage(RawMessage rawMsg)
        {
            Message msg;
            try
            {
                msg = await Message.DeserializeAsync(rawMsg.Data);
            }
            catch (Exception ex)
            {
                ErrorBlock.Post(new MessageProcessingError(rawMsg.Sender, null, ErrorType.ParseError, ex));
                return;
            }

            if (msg.IsResponse)
            {
                await NotifyResponseHandler(msg);
            }
            else
            {
                try
                {
                    await DistributeMessage(rawMsg.Sender, msg);
                }   
                catch (Exception ex)
                {
                    ErrorBlock.Post(new MessageProcessingError(rawMsg.Sender, null, ErrorType.Internal, ex));
                }
            }
        }

        private async Task NotifyResponseHandler(Message msg)
        {
            await _sem.WaitAsync();
            try
            {
                if (!_responseDic.ContainsKey(msg.ResponseId))
                {
                    //timeout has occoured -> handled by task waiting for response
                    return;
                }

                _responseDic[msg.ResponseId].Item2.SetResult(msg);
                _responseDic[msg.ResponseId].Item1.Dispose();
                _responseDic.Remove(msg.ResponseId);
            }
            finally
            {
                _sem.Release();
            } 
        }

        private async Task DistributeMessage(object sender,
                                             Message msg)
        { 
            if(!_messageHandlerDic.ContainsKey(msg.Module))
            {
                ErrorBlock.Post(new MessageProcessingError(sender, msg, ErrorType.UnhandledType, null));
                return;
            }

            try
            {
                await _messageHandlerDic[msg.Module].HandleMessage(sender, msg);
            }
            catch (Exception ex)
            {
                ErrorBlock.Post(new MessageProcessingError(sender, msg, ErrorType.HandlingException, ex));
            }
        }


        public IDisposable RegisterMessageHandler(IMessageHandler handler)
        {
            if(_messageHandlerDic.ContainsKey(handler.Module))
            {
                throw new ArgumentException("Message type is already handled!");
            }
            _messageHandlerDic.Add(handler.Module, handler);

            return new Unsubscriber<IMessageHandler>(handler, this);
        }

        public void Unsubscribe(IMessageHandler handler)
        {
            if (!_messageHandlerDic.ContainsKey(handler.Module))
            {
                throw new ArgumentException("Message type is not registered!");
            }
            _messageHandlerDic.Remove(handler.Module);
        }

        public Task<Message> WaitForResponseAsync(int responseId, TimeSpan timeout)
        {
            return WaitForResponseAsync(responseId, timeout, CancellationToken.None);
        }

        public async Task<Message> WaitForResponseAsync(int responseId, TimeSpan timeout, CancellationToken token)
        {
            TaskCompletionSource<Message> tcs = new();

            Action<Exception> abort = ex =>
            {
                tcs.TrySetException(ex);
                if (_responseDic.ContainsKey(responseId))
                {
                    _responseDic.Remove(responseId);
                }
            };

            token.Register(() =>
            {
                abort(new OperationCanceledException());
            });


            await _sem.WaitAsync();

            try
            {
                if (_responseDic.ContainsKey(responseId))
                {
                    throw new ArgumentException("Theres already a pending wait for this message!");
                }

                var timer = new Timer(_ =>
                {
                    abort(new TimeoutException());
                }, null, (int)timeout.TotalMilliseconds, Timeout.Infinite);

                _responseDic.Add(responseId, Tuple.Create(timer, tcs));
            }
            finally
            {
                _sem.Release();
            }
            return await tcs.Task;
        }

        public void Dispose()
        {
            if (_unsubscriber != null)
            {
                _unsubscriber.Dispose();
                _unsubscriber = null;
            }
        }


    }
}