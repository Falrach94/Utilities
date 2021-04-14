using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using ServerUtils.Endpoint_Manager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerUtils
{
    public delegate Task EndpointConnectedCallback(IEndpoint ep);
    public delegate Task EndpointDisconnectedCallback(IEndpoint ep, bool remoteDisconnect);

    public interface IEndpointObservable
    {
        List<IEndpoint> ConnectedEndpoints { get; }

        EndpointConnectedCallback EndpointConnectedHandler { get; set; }
        EndpointDisconnectedCallback EndpointDisconnectedHandler { get; set; }
    }
}