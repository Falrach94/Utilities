using MessageUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtils.MessageHandler
{
    public interface IMessageHandler
    {
        string Module { get; }

        Task HandleMessage(object sender, Message msg);
    }
}