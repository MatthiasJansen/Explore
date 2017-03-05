using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace Explore.Async.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var cts = new CancellationTokenSource();
                var task = MainAsync(cts.Token);

                Console.ReadKey();
                cts.Cancel();

                task.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task MainAsync(CancellationToken ct)
        {
            var addresses = (await Dns.GetHostAddressesAsync("microsoft.com")).Select(a => a.ToString()).ToArray();

            Console.WriteLine($"Found {addresses.Count()} addresses:");
            Console.WriteLine(string.Join(", ", addresses));
            
            var requestTasks = addresses.Select(a => $"http://{a}".GetStringAsync(ct));

            var result = await await Task.WhenAny(requestTasks);

            Console.WriteLine(result);
        }
    }
}