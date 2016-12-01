#region imports

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

#endregion

namespace Explore.Async.Exploration
{
    [TestFixture]
    [Category(@"Result")]
    public class ResultAccessExploration
    {
        private class UnitOfWork
        {
            public Task<string> WorkAsync()
            {
                var tcs = new TaskCompletionSource<string>();

                tcs.SetResult("Cookies are tasty!");

                return tcs.Task;
            }
        }

        [Test]
        public async Task AwaitResult()
        {
            var worker = new UnitOfWork();
            var result = worker.WorkAsync();

            //...

            await result;

            Assert.That(() => result,
                Is.Not.Null
                    .And.Not.Empty
                    .And.EqualTo("Cookies are tasty!"));
        }

        [Test]
        public void GetBlockingResult()
        {
            var worker = new UnitOfWork();
            var result = worker.WorkAsync().Result;

            Assert.That(() => result,
                Is.Not.Null
                    .And.Not.Empty
                    .And.EqualTo("Cookies are tasty!"));
        }
    }
}