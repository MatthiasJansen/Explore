using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explore.Linq.EFCore.Models;
using LinqKit;
using Microsoft.Extensions.DependencyInjection;
using Remotion.Linq.Clauses;

namespace Explore.Linq.EFCore.Demo
{
    public class BatchQueries
    {
        private static int[] SpecialCustomerIds => new[]
                                                   {
                                                       66,
                                                       147,
                                                       148,
                                                       294,
                                                       311,
                                                       408,
                                                       595,
                                                       607,
                                                       29557,
                                                       29583,
                                                       29663,
                                                       29696,
                                                       29699,
                                                       30031,
                                                       30035,
                                                   };

        public static void SimpleSelectByIds(IServiceScope scope)
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AdventureContext>();

            var customers = ctx.Customer.Where(_ => SpecialCustomerIds.Any(customerId => _.CustomerId == customerId));

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId}: {customer.FirstName} {customer.LastName}");
            }
        }

        public static void SimpleSelectByIdsWithInlining1(IServiceScope scope)
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AdventureContext>();

            var predicate = SpecialCustomerIds.Aggregate(PredicateBuilder.New<Customer>(false),
                                                         (p, id) => p.Or(c => c.CustomerId == id));

            var customers = ctx.Customer.Where(predicate);

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId}: {customer.FirstName} {customer.LastName}");
            }
        }

        public static void SimpleSelectByIdsWithInlining2(IServiceScope scope)
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AdventureContext>();

            var customers = ctx.Customer
                               .AsExpandable()
                               .Where(_ => SpecialCustomerIds.Any(customerId => _.CustomerId == customerId));

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId}: {customer.FirstName} {customer.LastName}");
            }
        }
    }
}