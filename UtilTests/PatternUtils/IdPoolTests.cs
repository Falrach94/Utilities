using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternUtils.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UtilTests.PatternUtilsTests
{
    [TestClass]
    public class IdPoolTests
    {
        [TestMethod]
        public void TestIdPool()
        {
            IdPool pool = new();

            SortedList<int, int> returned = new();

            List<IDisposable> ids = new();
            for(int i = 0; i < 100; i++)
            {
                int id;
                var disp = pool.GetNextId(out id);

                Assert.IsTrue(id == i, "new id order");

                ids.Add(disp);
            }

            for(int i = 99; i >= 0; i -= 9)
            {
                ids[i].Dispose();
                returned.Add(i, i);
            }

            Assert.ThrowsException<ObjectDisposedException>(() =>
            {
                ids[0].Dispose();
            });

            int iLast = -1;
            for(int i = 0; i < 20; i++)
            {
                pool.GetNextId(out int id);

                if (returned.Count > 0)
                {
                    Assert.AreEqual(returned.First().Key, id, "is returned id");
                    returned.RemoveAt(0);
                }
                Assert.IsTrue(iLast < id, "id order");
                iLast = id;
            }
            Assert.IsTrue(TestIdPool_parallel(pool).Wait(1000));
        }

        private static async Task TestIdPool_parallel(IdPool pool)
        {
            const int COUNT = 10000;
            List<Task> tasks = new();
            TaskCompletionSource tcs = new();
            SemaphoreSlim sem = new(1, 1);
            HashSet<int> ids = new();

            for (int i = 0; i < COUNT; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await tcs.Task; //synchronize tasks
                    _ = pool.GetNextId(out int id);
                    await sem.WaitAsync();
                    try
                    {
                        Assert.IsFalse(ids.Contains(id), $"id {id} was given out multiple times");
                        ids.Add(id);
                    }
                    finally
                    {
                        sem.Release();
                    }
                })); 
            }
            tcs.SetResult();
            await Task.WhenAll(tasks);
        }
    }
}
