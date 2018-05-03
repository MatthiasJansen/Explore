using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explore.Linq.EFCore.Demo
{
    public static class Projections
    {
        public static void GroupingAndSortingFluent(IServiceScope scope)
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AdventureContext>();

            var projectedProductGroups = ctx.Product.Include(_ => _.ProductCategory)
                                            .GroupBy(_ => _.ProductCategory.Name)
                                            .Select(_ => new
                                                         {
                                                             Category = _.Key,
                                                             Products = _.OrderBy(p => p.ListPrice)
                                                         });

            foreach (var productCategory in projectedProductGroups)
            {
                Console.WriteLine($"{productCategory.Category} contains:");
                foreach (var product in productCategory.Products)
                {
                    Console.WriteLine($"    {product.Name} costs: {product.ListPrice}");
                }
            }
        }

        public static void GroupingAndSortingLinq(IServiceScope scope)
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AdventureContext>();


            var projectedProductGroups = from p in ctx.Product
                                         group p by p.ProductCategory.Name
                                         into c
                                         select new
                                                {
                                                    Category = c.Key,
                                                    Products = c.OrderBy(p => p.ListPrice)
                                                };


            foreach (var productCategory in projectedProductGroups)
            {
                Console.WriteLine($"{productCategory.Category} contains:");
                foreach (var product in productCategory.Products)
                {
                    Console.WriteLine($"    {product.Name} costs: {product.ListPrice}");
                }
            }
        }
    }
}