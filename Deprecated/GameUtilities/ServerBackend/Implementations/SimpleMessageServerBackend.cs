using GameManagement;
using PlayerModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.ServerBackend
{
    public delegate Task GameMessageHandler(int playerId, object data);
    public delegate Task GameMessageHandler<T>(int playerId, T data);
    public abstract class SimpleMessageServerBackend : ServerGameBackend
    {
        private readonly Dictionary<string, GameMessageHandler> _messageDic = new Dictionary<string, GameMessageHandler>();

        public SimpleMessageServerBackend()
        {
            RegisterMessages();
        }

        public override Task ProccessMessage(object data)
        {
            int playerId;
            string msg;
            object content;
            (playerId, msg, content) = (Tuple<int, string, object>)data;

            _messageDic[msg].Invoke(playerId, content);

            return Task.CompletedTask;
        }

        protected Task Broadcast(string msg, object data = null)
        {
            return  Messenger.Broadcast(Tuple.Create(msg, data));
        }
        protected Task Send(Player p, string msg, object data = null)
        {
            return Messenger.Send(p, Tuple.Create(msg, data));
        }

        protected abstract void RegisterMessages();

        protected void RegisterMessage<T>(string name, GameMessageHandler<T> handler)
        {
            _messageDic.Add(name, (p,o) => handler(p,(T)o));
        }

    }
}
