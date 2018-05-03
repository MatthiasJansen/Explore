using System.Linq;

namespace Explore.Linq.EFCore.Demo
{
    public static class Misc
    {
        public static void Comparison()
        {
            var enumerableNumbers = Enumerable.Range(0, 10);
            var queryableNumbers = Enumerable.Range(0, 10).AsQueryable();

            var enumerableResult = enumerableNumbers.Where(n => n > 4);
            var queryableResult = queryableNumbers.Where(n => n > 4);
        }
    }
}