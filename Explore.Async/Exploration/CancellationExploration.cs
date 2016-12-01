#region imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;

#endregion imports

namespace Explore.Async.Exploration
{
    [TestFixture][Category(@"Cancellation")]
    public class CancellationExploration
    {
        public interface IAsyncWorker
        {
            Task WorkAsync(int workAmount, CancellationToken ct);
        }

        private class MockAsyncWorker1 : IAsyncWorker
        {
            public async Task WorkAsync(int workAmount, CancellationToken ct)
            {
                await Task.Delay(workAmount);
            }
        }

        private class MockAsyncWorker2 : IAsyncWorker
        {
            public async Task WorkAsync(int workAmount, CancellationToken ct)
            {
                await Task.Delay(workAmount, ct);
            }
        }

        private class MockAsyncWorker3 : IAsyncWorker
        {
            public async Task WorkAsync(int workAmount, CancellationToken ct)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(workAmount);
            }
        }

        private static IConstraint CreateExceptionConstraint(Type exception)
        {
            if (exception == null)
            {
                return Throws.Nothing;
            }

            return Throws.InstanceOf(exception);
        }

        [TestCase(typeof(MockAsyncWorker1), null)]
        [TestCase(typeof(MockAsyncWorker2), typeof(OperationCanceledException))]
        [TestCase(typeof(MockAsyncWorker3), typeof(OperationCanceledException))]
        public void AsyncCancellation(Type workerType, Type exceptionType)
        {
            var workAmount = 50;
            var worker = (IAsyncWorker)Activator.CreateInstance(workerType);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () => await worker.WorkAsync(workAmount, cts.Token), CreateExceptionConstraint(exceptionType));
        }

        [TestCaseSource(nameof(GetParallelImmediateCancellationTestCases))]
        public void ParallelImmediateCancellation(IAsyncWorker worker)
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () =>
            {
                return await Task.Factory.StartNew(async () =>
                    {
                        await worker.WorkAsync(50, cts.Token);
                    }, cts.Token, TaskCreationOptions.PreferFairness | TaskCreationOptions.HideScheduler, TaskScheduler.Default);
            }, Throws.InstanceOf<TaskCanceledException>());
        }

        private static IEnumerable<TestCaseData> GetParallelImmediateCancellationTestCases()
        {
            yield return new TestCaseData(new MockAsyncWorker1());
            yield return new TestCaseData(new MockAsyncWorker2());
            yield return new TestCaseData(new MockAsyncWorker3());
        }

        [TestCaseSource(nameof(GetParallelDelayedCancellationTestCases))]
        public void ParallelDelayedCancellation(IAsyncWorker worker, int workAmount, Type exceptionType)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            Assert.That(async () =>
                {
                    await Task.Run(async () => await worker.WorkAsync(workAmount, cts.Token), cts.Token);
                },
                CreateExceptionConstraint(exceptionType));
        }

        private static IEnumerable<TestCaseData> GetParallelDelayedCancellationTestCases()
        {
            yield return new TestCaseData(new MockAsyncWorker1(), 500, (Type) null).SetName("Mock1, 500, null");
            yield return new TestCaseData(new MockAsyncWorker2(), 500, typeof(OperationCanceledException));
            yield return new TestCaseData(new MockAsyncWorker3(), 500, (Type) null);

            yield return new TestCaseData(new MockAsyncWorker1(), 1000, (Type)null);
            yield return new TestCaseData(new MockAsyncWorker2(), 1000, typeof(OperationCanceledException));
            yield return new TestCaseData(new MockAsyncWorker3(), 1000, (Type)null);
        }
    }
}