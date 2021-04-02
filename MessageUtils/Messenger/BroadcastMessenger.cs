using MessageUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageUtils.Messenger
{
    public class BroadcastMessenger : MultiTargetMessenger, IBroadcastMessenger
    {
        public BroadcastMessenger(IMessageProcessor messageProcessor) : base(messageProcessor)
        {
        }

        public Task BroadcastMessageAsync(IEnumerable<IMessageSender> receiver, Message msg)
        {
            List<Task> tasks = new();

            foreach(var r in receiver)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await SendMessageAsync(r, new Message(msg));
                    }
                    catch (SendFailedException) { }
                }));
            }

            return Task.WhenAll(tasks);
        }

        public async Task<List<Message>> BroadcastMessageAndWaitAsync(IEnumerable<IMessageSender> receiver, Message msg, int timeoutMs)
        {
            List<Task> tasks = new();

            List<Message> responseList = new();
            SemaphoreSlim sem = new(1, 1);

            foreach (var r in receiver)
            {
                tasks.Add(Task.Run(async () =>
                {
                    Message response;
                    try
                    {
                        response = await SendAndWaitForResponseAsync(r, new Message(msg), timeoutMs);
                    }
                    catch
                    {
                        response = null;
                    }
                    await sem.WaitAsync();
                    responseList.Add(response);
                    sem.Release();
                }));
            }

            await Task.WhenAll(tasks);

            return responseList;
        }
    }
}
