using MessageUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtils.Messenger
{
    public interface IMessageSender
    {
        Task<bool> SendAsync(Message msg);
    }
}
