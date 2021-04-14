using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncUtils
{
    public static class AsyncUtilities
    {
        public static Task RepeatAsync(Func<Task> task, int count)
        {
            List<Task> tasks = new();
            for (int i = 0; i < count; i++)
            {
                tasks.Add(Task.Run(task));
            }
            return Task.WhenAll(tasks);
        }
        public static Task ForEachAsync<T>(Func<T, Task> task, IEnumerable<T> collection)
        {
            List<Task> tasks = new();
            foreach(var obj in collection)
            {
                tasks.Add(Task.Run(() => task(obj)));
            }
            return Task.WhenAll(tasks);
        }
    }
}
