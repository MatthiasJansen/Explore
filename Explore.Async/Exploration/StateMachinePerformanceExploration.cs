#region imports

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

#endregion

namespace Explore.Async.Exploration
{
    [TestFixture]
    [Explicit("Long running set of explorations.")]
    [Category(@"Measure")]
    public class StateMachinePerformanceExploration
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        public async Task MeasureAwaitingNonCpuBoundWorkForN(int n)
        {
            var ids = Enumerable.Range(1, n);

            var tasks = ids.Select(async id =>
            {
                await Task.Delay(1000);
                return id;
            }).ToList();

            var sw = Stopwatch.StartNew();

            await Task.WhenAll(tasks);

            sw.Stop();

            Console.WriteLine(sw.Elapsed);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        public async Task MeasureAwaitingNonCpuBoundWorkWithBarrierForN(int n)
        {
            var tcs = new TaskCompletionSource<bool>();

            var ids = Enumerable.Range(1, n);

            var tasks = ids.Select(async id =>
            {
                await tcs.Task;
                await Task.Delay(1000);
                return id;
            }).ToList();

            var sw = Stopwatch.StartNew();

            tcs.SetResult(true);

            await Task.WhenAll(tasks);

            sw.Stop();

            Console.WriteLine(sw.Elapsed);
        }
    }
}