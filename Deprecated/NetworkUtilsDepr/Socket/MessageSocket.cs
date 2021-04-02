using System;
using System.IO;
using System.Threading;

namespace NetworkUtils.Socket
{
    public delegate void MessageParser(Stream stream);
    public class MessageSocket : Socket
    {
        #region fields
        private CancellationTokenSource _cancelSource;
        private Thread _messageThread;
        #endregion

        #region properties
        public MessageParser ParseMessageCallback { get; set; }
        #endregion


        public MessageSocket()
        {
        }

        protected override void Start()
        {
            base.Start();
            _cancelSource = new CancellationTokenSource();
            _messageThread = new Thread(HandleStream);
            _messageThread.Start();
        }
        protected override void Stop()
        {
            base.Stop();
            _cancelSource.Cancel();
            lock (Stream)
            {
                Monitor.PulseAll(Stream);
            }
        }

        private void HandleStream()
        {
            try
            {
                long lastLen = 0;
                long lastPos = 0;
                while (!_cancelSource.Token.IsCancellationRequested)
                {
                    lock (Stream)
                    {
                        while (Stream.Length == lastLen)
                        {
                            Monitor.Wait(Stream);
                            Log.Trace("new message on stream (" + Stream.Length + " Bytes)");
                            if (_cancelSource.Token.IsCancellationRequested)
                            {
                                return;
                            }
                        }
                        Stream.Position = lastPos;
                        try
                        {
                            while (Stream.Length != Stream.Position)
                            {
                                ParseMessageCallback?.Invoke(Stream);
                                lastPos = Stream.Position;
                            }
                            Stream.SetLength(0);
                            lastLen = 0;
                            lastPos = 0;
                        }
                        catch
                        {
                            lastLen = Stream.Length;
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                //stream closed
            }
        }
    }
}
