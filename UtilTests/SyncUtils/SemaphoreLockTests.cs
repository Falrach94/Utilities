using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UtilTests.SyncUtils
{
    [TestClass]
    public class SemaphoreLockTests
    {
        [TestMethod]
        public void TestSemaphoreLock_MutualExclusion()
        {
            const int TASK_NUM = 1000;
            SemaphoreLock semLock = new();

            DateTime lastStart = DateTime.Now, lastEnd = DateTime.Now;

            List<Task> tasks = new();

            Stopwatch watch = new();
            Stopwatch workWatch = new();
            watch.Start();



            for (int i = 0; i < TASK_NUM; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var sL = await semLock.LockAsync();

                    Assert.IsTrue(lastEnd >= lastStart, "assert locked region");

                    lastStart = DateTime.Now;
                    workWatch.Start();
                    for (int j = 0; j < 100000; j++) ;
                    workWatch.Stop();
                    lastEnd = DateTime.Now;

                }));
            }
            var allTask = Task.WhenAll(tasks);
            allTask.Wait();

            watch.Stop();

            Assert.IsTrue(watch.ElapsedMilliseconds - workWatch.ElapsedMilliseconds < TASK_NUM, "assert context time");

        }

        [TestMethod]
        public void TestSemaphoreLock_Cancel()
        {
            SemaphoreLock semLock = new();

            CancellationTokenSource cts = new();

            var lockTask = semLock.LockAsync();
            lockTask.Wait();
            var lockToken = lockTask.Result;

            var cancelTask = Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
            {
                using var token = await semLock.LockAsync(cts.Token);
            }, "operation canceled exception");

            Task.Delay(50).Wait();

            cts.Cancel();

            Assert.IsTrue(cancelTask.Wait(10), "cancel within timeout");

            semLock.ReturnToken(lockToken);


        }

        [TestMethod]
        public void TestSemaphoreLock_Timeout()
        {
            TimeSpan Timeout = TimeSpan.FromMilliseconds(10);
            SemaphoreLock semLock = new();

            CancellationTokenSource cts = new();

            var lockTask = semLock.LockAsync();
            lockTask.Wait();
            var lockToken = lockTask.Result;

            var timeoutTask = Assert.ThrowsExceptionAsync<TimeoutException>(async () =>
            {
                using var token = await semLock.LockAsync(Timeout, cts.Token);
            }, "operation timeout exception");

            Task.Delay(50).Wait();

            Assert.IsTrue(timeoutTask.IsCompleted, "timeout occoured");

            cts.Cancel();

            semLock.ReturnToken(lockToken);

        }


    }
}
