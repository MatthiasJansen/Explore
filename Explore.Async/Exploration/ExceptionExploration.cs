#region imports

using System;
using System.Linq;
using System.Threading.Tasks;
using Explore.Async.Exploration.Exceptions;
using NUnit.Framework;

#endregion

namespace Explore.Async.Exploration
{
    [TestFixture]
    [Category("Exception")]
    public class ExceptionExploration
    {
        [TestCase(typeof(FatalUnrecoverableUserErrorException))]
        [TestCase(typeof(FatalUnrecoverableUserErrorException), typeof(CriticalUnrecoverableUserErrorException))]
        public void AggregateExceptionExploration(params Type[] exceptionTypes)
        {
            var action = new TestDelegate(() =>
                Parallel.ForEach(exceptionTypes, e =>
                    {
                        var exception = (Exception) Activator.CreateInstance(e);
                        throw exception;
                    }
                ));

            Assert.That(action, Throws.Exception.InstanceOf<AggregateException>());
        }

        [TestCase(typeof(CriticalUnrecoverableUserErrorException))]
        [TestCase(typeof(CriticalUnrecoverableUserErrorException), typeof(FatalUnrecoverableUserErrorException))]
        public void RegularExceptionExploration(params Type[] exceptionTypes)
        {
            var action = new AsyncTestDelegate(async () =>
                {
                    var tasks = exceptionTypes.Select(e =>
                    {
                        var tcs = new TaskCompletionSource<bool>();
                        var exception = (Exception) Activator.CreateInstance(e);
                        tcs.SetException(exception);
                        return tcs.Task;
                    });

                    await Task.WhenAll(tasks);
                }
            );

            Assert.That(action, Throws.Exception.Not.InstanceOf<AggregateException>());
        }

        [TestCase][Category("Exception-Resurface-Behaviour")]
        public void AsyncThrowBehaviourExploration1()
        {
            var task = default(Task);

            Assert.That(() =>
            {
                task = Task.Run(async () =>
                {
                    await Task.Delay(50);

                    throw new FatalUnrecoverableUserErrorException();
                });
            }, Throws.Nothing);

            Assert.That(async () => { await Task.Delay(100); }, Throws.Nothing);

            Assert.That(async () => { await task; }, Throws.Exception.InstanceOf<FatalUnrecoverableUserErrorException>());
        }

        [TestCase]
        [Category("Exception-Resurface-Behaviour")]
        public void AsyncThrowBehaviourExploration2()
        {
            var task = default(Task);

            Assert.That(() =>
            {
                task = Task.Run(async () =>
                {
                    await Task.Delay(50);

                    throw new FatalUnrecoverableUserErrorException();
                });
            }, Throws.Nothing);

            Assert.That(async () => { await Task.Delay(100); }, Throws.Nothing);

            Assert.That(() => { task.Wait(); }, Throws.Exception.InstanceOf<AggregateException>()
                .And
                .InnerException.InstanceOf<FatalUnrecoverableUserErrorException>());
        }

        [TestCase]
        [Category("Exception-Resurface-Behaviour")]
        public void AsyncThrowBehaviourExploration3()
        {
            var task = default(Task<bool>);

            Assert.That(() =>
            {
                task = Task.Run(async () =>
                {
                    await Task.Delay(50);

                    throw new FatalUnrecoverableUserErrorException();

#pragma warning disable 162
                    return true;
#pragma warning restore 162
                });
            }, Throws.Nothing);

            Assert.That(async () => { await Task.Delay(100); }, Throws.Nothing);

            Assert.That(() =>
            {
                var result = task.Result;
            }, Throws.Exception.InstanceOf<AggregateException>()
                .And
                .InnerException.InstanceOf<FatalUnrecoverableUserErrorException>());
        }
    }
}