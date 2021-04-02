using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtils.Endpoint
{
    public record RawMessage
    {
        public RawMessage(object sender, byte[] data)
        {
            Sender = sender;
            Data = data;
        }

        public object Sender { get; }
        public byte[] Data { get; }
    }
}
