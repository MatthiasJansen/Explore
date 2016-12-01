#region imports

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

#endregion

namespace Explore.Async.Exploration
{
    [TestFixture]
    [Category("Bridge")]
    public class BridgeWithApmExploration
    {
        [Test]
        public async Task BridgeFromApmToAsync()
        {
            var stream = new MemoryStream(Encoding.Unicode.GetBytes("Nom, nom, nom."));

            var buffer = new byte[6];

            var task = Task.Factory.FromAsync(
                (acb, o) => stream.BeginRead(buffer, 0, buffer.Length, acb, o),
                (result) => stream.EndRead(result),
                null
            );

            await task;

            Console.Write(Encoding.Unicode.GetString(buffer));
        }
    }
}