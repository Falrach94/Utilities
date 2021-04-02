using System.Threading.Tasks;

namespace PatternUtils.Module_Framework
{
    public interface IInterfaceProvider
    {
        /// <summary>
        /// Returns requested interface in a threadsafe manner.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InterfaceNotFoundException"></exception>
        Task<T> GetInterfaceAsync<T>();

    }
}