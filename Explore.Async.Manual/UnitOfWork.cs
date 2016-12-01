using System;
using System.Threading.Tasks;

namespace Explore.Async.Manual
{
    public class UnitOfWork
    {

        public async Task WorkAsync()
        {
            Console.WriteLine($"Entered: {nameof(WorkAsync)}");

            await Task.Delay(50);

            Console.WriteLine("Waited for 50ms.");

            await Task.Delay(100);

            Console.WriteLine($"Leaving: {nameof(WorkAsync)}");
        }
    }
}
