﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkUtils.Socket
{
    public static class SocketEx
    {
        public async static Task<bool> ReadUntilCompleteAsync(this Stream stream, byte[] buffer, CancellationToken cancelToken)
        {
            int count;
            int i = 0;
            try
            {
                while ((count = await stream.ReadAsync(buffer.AsMemory(i), cancelToken)) != 0)
                {
                    i += count;
                    if (i == buffer.Length)
                    {
                        return true;
                    }
                }
            }
            catch (OperationCanceledException) { }

            return false;
        }
    }
}
