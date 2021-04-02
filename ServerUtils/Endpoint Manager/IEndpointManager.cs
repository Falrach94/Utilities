using NetworkUtils.Endpoint;
using ServerUtils.Endpoint_Manager;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerUtils
{
    /// <summary>
    /// Manages tcp client connections and gives out an event for each connect or disconnect.
    /// IncomingClientBlock should be connected to a TcpClientListener
    /// Bundles incoming raw messages into RawMessageReceivedBlock.
    /// </summary>
    public interface IEndpointManager : IRawMessageReceiver
    {
        ITargetBlock<TcpClient> IncomingClientBlock { get; }
        List<IEndpoint> ConnectedEndpoints { get; }

        event EventHandler<EndpointChangedEventArgs> EndpointConnectionChanged;

        /// <summary>
        /// Disconnects the passed endpoint and waits for the disconnect to be processed
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public Task DisconnectEndpointAsync(IEndpoint endpoint);

        public Task DisconnectAllAsync();

    }
}