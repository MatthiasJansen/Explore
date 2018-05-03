using System;
using System.Linq;

namespace Explore.Linq.EFCore.Demo
{
    public static class Immutability
    {
        public static void ShowImmutability()
        {
            var numbers = Enumerable.Range(0, 10);
            Console.WriteLine($"Numbers in the original sequence: {numbers.Count()}");

            var filter1 = numbers.Where(n => n > 4);
            Console.WriteLine($"Remaining numbers: {filter1.Count()}");

            var filter2 = filter1.Where(n => n > 4);
            Console.WriteLine($"Remaining numbers: {filter2.Count()}");
            
            Console.WriteLine($"Are these two enumerables reference equal? {(Equals(filter1, filter2) ? "yep" : "nah")}");
        }
    }
}
