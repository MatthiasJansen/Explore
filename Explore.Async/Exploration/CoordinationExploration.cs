#region imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

#endregion imports

namespace Explore.Async.Exploration
{
    [TestFixture][Explicit][Category(@"Coordination")]
    public class CoordinationExploration
    {
        private static readonly object lockObj = new object();

        private class Counter : IDisposable
        {
            private static uint globalCount = 0;

            public Counter()
            {
                globalCount++;
            }

            public static uint Count => globalCount;

            public void Dispose()
            {
                globalCount--;
            }
        }


        [TestCase(50)]
        public async Task Concurrent(int n)
        {
            var tcs = new TaskCompletionSource<bool>();

            var tasks = Enumerable.Range(1, n).Select(i => Task.Run(async () =>
            {
                await tcs.Task;

                using (new Counter())
                {
                    await Task.Delay(50);
                }

                return new Tuple<int, uint>(i, Counter.Count);
            })).ToArray();

            tcs.SetResult(true);

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                TestContext.WriteLine($"Task #{result.Item1}, Access count: {result.Item2}");
            }

            Assert.That(() => results, Has.All.Property(nameof(Tuple<int, uint>.Item2)).EqualTo(0));
        }

        [TestCase(50)]
        public async Task Asynchron(int n)
        {
            var tcs = new TaskCompletionSource<bool>();

            var tasks = Enumerable.Range(1, n).Select(async i =>
            {
                await tcs.Task;

                using (new Counter())
                {
                    await Task.Delay(50);
                }

                return new Tuple<int, uint>(i, Counter.Count);
            }).ToArray();

            tcs.SetResult(true);

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                TestContext.WriteLine($"Task #{result.Item1}, Access count: {result.Item2}");
            }

            Assert.That(() => results, Has.All.Property(nameof(Tuple<int, uint>.Item2)).EqualTo(0));
        }

        [TestCase(50)]
        public async Task AsynchronWithSemaphore(int n)
        {
            var tcs = new TaskCompletionSource<bool>();
            var semaphore = new SemaphoreSlim(1, 1);

            var tasks = Enumerable.Range(1, n).Select(async i =>
            {
                await tcs.Task;
                await semaphore.WaitAsync();

                using (new Counter())
                {
                    await Task.Delay(50);
                }

                semaphore.Release();

                return new Tuple<int, uint>(i, Counter.Count);
            }).ToArray();

            tcs.SetResult(true);

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                TestContext.WriteLine($"Task #{result.Item1}, Access count: {result.Item2}");
            }

            Assert.That(() => results, Has.All.Property(nameof(Tuple<int, uint>.Item2)).EqualTo(0));
        }


        [TestCase(50)]
        public async Task ConcurrentWithLock(int n)
        {
            var tcs = new TaskCompletionSource<bool>();


            var tasks = Enumerable.Range(1, n).Select(i => Task.Run(async () =>
            {
                await tcs.Task;
                lock(lockObj)
                {
                    using (new Counter())
                    {
                        //await Task.Delay(50);
                    }
                }

                return new Tuple<int, uint>(i, Counter.Count);
            })).ToArray();

            tcs.SetResult(true);

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                TestContext.WriteLine($"Task #{result.Item1}, Access count: {result.Item2}");
            }

            Assert.That(() => results, Has.All.Property(nameof(Tuple<int, uint>.Item2)).EqualTo(0));
        }
    }
}