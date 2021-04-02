using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilsLib
{
    public static class RandomEx
    {
        //        private static Dictionary<Random, SemaphoreSlim> _randDic = new Dictionary<Random, SemaphoreSlim>();

        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1,1); //TODO: replace lazy implementation

        public static async Task<double> NextDoubleTSAsync(this Random rand)
        {
            await _semaphore.WaitAsync();
            double val = rand.NextDouble();
            _semaphore.Release();
            return val;
        }
        public static double NextDoubleTS(this Random rand)
        {
            _semaphore.Wait();
            double val = rand.NextDouble();
            _semaphore.Release();
            return val;
        }
    }
}
