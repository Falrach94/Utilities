using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace UtilTests.ServerUtils.Mocks
{
    public class MockListener
    {
        public BufferBlock<TcpClient> Block
        {
            get;
        } = new();



    }
}
