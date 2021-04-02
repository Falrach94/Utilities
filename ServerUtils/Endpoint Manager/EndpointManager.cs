﻿using MessageUtils.Message_Socket;
using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using NetworkUtils.Tcp_Client_Listener;
using ServerUtils.Endpoint_Manager;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerUtils
{
    /// Communication is expected to be performed via a PacketSocket with preceeding buffer size.
    public class EndpointManager : IEndpointManager
    {
        private readonly SemaphoreLock _lock = new(); 

        private readonly ActionBlock<TcpClient> _incomingClientHandlerBlock;
        private readonly Dictionary<IEndpoint, IDisposable> _linkDic = new();

        private readonly Dictionary<IEndpoint, TaskCompletionSource> _disconnectDic = new();

        public BufferBlock<RawMessage> RawMessageBlock { get; } = new();
        public ITargetBlock<TcpClient> IncomingClientBlock => _incomingClientHandlerBlock;
        public List<IEndpoint> ConnectedEndpoints => new List<IEndpoint>(_linkDic.Keys);

        ISourceBlock<RawMessage> IRawMessageReceiver.RawMessageBlock => RawMessageBlock;


        public event EventHandler<EndpointChangedEventArgs> EndpointConnectionChanged;

        public EndpointManager()
        {
            _incomingClientHandlerBlock = new ActionBlock<TcpClient>(HandleIncomingClient);
        }

        /// <summary>
        /// register new endpoint and propagate event
        /// </summary>
        /// <param name="endpoint"></param>
        private void HandleIncomingClient(TcpClient client)
        {
            if (!client.Connected)
            {
                return;
            }

            var endpoint = new MessageEndpoint(new PacketSocket(client));

            var task = _lock.LockAsync();
            task.Wait();
            using (var l = task.Result)
            {

                //bundle messages from all endpoints to one output block
                var linkDisposer = endpoint.RawMessageBlock.LinkTo(RawMessageBlock);

                //memorize link disposer for disconnect event
                _linkDic.Add(endpoint, linkDisposer);

                //register to disconnect event
                endpoint.Disconnected += Endpoint_Disconnected;
            }

            //propagate event
            EndpointConnectionChanged?.Invoke(this, EndpointChangedEventArgs.NewConnection(endpoint));
        }


        /// <summary>
        /// remove all links to the disconnected endpoint and propagate event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Endpoint_Disconnected(object sender, DisconnectedArgs e)
        {
            var endpoint = (MessageEndpoint)sender;

            var task = _lock.LockAsync();
            task.Wait();
            using (var l = task.Result)
            {
                //remove disconnect event
                endpoint.Disconnected -= Endpoint_Disconnected;

                //remove message link
                _linkDic[endpoint].Dispose();
                _linkDic.Remove(endpoint);

                if(_disconnectDic.ContainsKey(endpoint))
                {
                    _disconnectDic[endpoint].SetResult();
                    _disconnectDic.Remove(endpoint);
                }
            }
            //propagate event
            EndpointConnectionChanged?.Invoke(this, EndpointChangedEventArgs.Disconnect(endpoint, e));
        }

        /// <summary>
        /// Disconnects the passed endpoint and waits for the disconnect to be processed
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">endpoint not registered on this manager</exception>
        public async Task DisconnectEndpointAsync(IEndpoint endpoint)
        {
            if (endpoint is null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            var tcs = new TaskCompletionSource();

            using (var l = await _lock.LockAsync())
            {
                if (!_linkDic.ContainsKey(endpoint))
                {
                    throw new ArgumentException("Can't disconnect unregistered endpoint!");
                }
                _disconnectDic.Add(endpoint, tcs);
            }
            await endpoint.Socket.DisconnectAsync();
            await tcs.Task;
        }

        public async Task DisconnectAllAsync()
        {
            var endpoints = new List<IEndpoint>(ConnectedEndpoints);
            foreach(var ep in endpoints)
            {
                await DisconnectEndpointAsync(ep);
            }
        }
    }
}
