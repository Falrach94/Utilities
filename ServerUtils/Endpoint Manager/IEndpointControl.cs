using NetworkUtils.Endpoint;
using System.Threading.Tasks;

namespace ServerUtils
{
    public interface IEndpointControl
    {
        /// <summary>
        /// Disconnects the passed endpoint and waits for the disconnect to be processed
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public Task DisconnectEndpointAsync(IEndpoint endpoint);

        public Task DisconnectAllAsync();
    }
}