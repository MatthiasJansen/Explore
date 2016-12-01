#region imports

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

#endregion imports

namespace Explore.Async.Exploration
{
    [TestFixture]
    public class CompositionExploration
    {
        [Test]
        public async Task WhenAny()
        {
            var tcs1 = new TaskCompletionSource<string>();

            var tcs2 = new TaskCompletionSource<string>();
            tcs2.SetResult("done.");

            await Task.WhenAny(tcs1.Task, tcs2.Task);
        }

        [Test]
        public async Task WhenAll()
        {
            await Task.WhenAll(Task.Delay(5), Task.Delay(100));
        }
    }
}