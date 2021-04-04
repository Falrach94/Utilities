using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncUtils.Barrier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTests.SyncUtils
{
    [TestClass]
    public class AsyncBarrierTests
    {
        [TestMethod]
        public void TestAsyncBarrier()
        {
            const int COUNT = 5;

            AsyncBarrier barrier = new(COUNT);

            Assert.IsTrue(barrier.CurrentCount == 0);
            Assert.IsTrue(barrier.Remaining == COUNT);
            Assert.IsTrue(barrier.TargetCount == COUNT);

            List<Task> taskList = new();

            for(int i = 0; i < COUNT; i++)
            {
                var task = Task.Run(async () =>
                {
                    await barrier.SignalAndWaitAsync();
                });
                if (i != COUNT - 1)
                {
                    Assert.IsFalse(task.Wait(50));
                    Assert.IsTrue(barrier.Remaining == COUNT - i - 1);
                }
                taskList.Add(task);
            }
            Assert.IsTrue(Task.WhenAll(taskList).Wait(50));

        }
    }
}
