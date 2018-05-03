using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explore.Linq.EFCore.Demo
{
    public static class QueryRelatedData
    {
        public static void WithoutInclude(IServiceScope scope)
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AdventureContext>();

            var products = ctx.Product;

            foreach (var product in products)
            {
                Console.WriteLine($"{product.Name} is in category: {product.ProductCategory?.Name}");
            }
        }

        public static void WithInclude(IServiceScope scope)
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AdventureContext>();

            var products = ctx.Product.Include(_ => _.ProductCategory);

            foreach (var product in products)
            {
                Console.WriteLine($"{product.Name} is in category: {product.ProductCategory?.Name}");
            }
        }
    }
}