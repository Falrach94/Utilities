using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtils.Socket
{
    public static class SocketEx
    {
        public async static Task<bool> ReadUntilCompleteAsync(this Stream stream, byte[] buffer)
        {
            int count;
            int i = 0;
            while ((count = await stream.ReadAsync(buffer.AsMemory(i))) != 0)
            {
                i += count;
                if(i == buffer.Length)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
