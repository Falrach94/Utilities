using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageUtils.Messenger
{
    public struct Error
    {
        public string Message { get; }
        public int Type { get; }

        public Error(int errorType, string msg)
        {
            Message = msg;
            Type = errorType;
        }
    }
}
