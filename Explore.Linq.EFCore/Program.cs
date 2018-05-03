using System;
using System.Linq;
using Explore.Linq.EFCore.Demo;
using Explore.Linq.EFCore.Models;
using Explore.Linq.EFCore.Options;
using Explore.Linq.EFCore.Scaffolding;
using LinqKit;
using Microsoft.Extensions.DependencyInjection;

namespace Explore.Linq.EFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                           .Configure<DatabaseConnectionOptions>
                               (_ =>
                                {
                                    _.Srv = args[0];
                                    _.Cat = args[1];
                                    _.Usr = args[2];
                                    _.Pwd = args[3];
                                })
                           .AddDbContextWithConfiguration();


            var provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                //SimpleStartsWithQuery(scope);
                //BatchQueries.SimpleSelectByIds(scope);
                //BatchQueries.SimpleSelectByIdsWithInlining1(scope); // +
                //BatchQueries.SimpleSelectByIdsWithInlining2(scope); // -

                //QueryRelatedData.WithoutInclude(scope);
                //QueryRelatedData.GroupingAndSorting1(scope);

                //Projections.GroupingAndSortingFluent(scope);
                Projections.GroupingAndSortingLinq(scope);

                //Immutability.ShowImmutability();
            }

            

            Console.ReadKey();
        }



        public static void SimpleStartsWithQuery(IServiceScope scope)
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AdventureContext>();

            var customers = ctx.Customer
                               .Where(_ => _.FirstName.StartsWith("Al"));

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId}: {customer.FirstName} {customer.LastName}");
            }
        }


    }
}