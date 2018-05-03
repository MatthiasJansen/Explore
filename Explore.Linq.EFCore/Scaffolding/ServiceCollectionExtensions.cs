using System;
using System.Data.Common;
using Explore.Linq.EFCore.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Explore.Linq.EFCore.Scaffolding
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContextWithConfiguration(this IServiceCollection services)
        {
            return services.AddEntityFrameworkSqlServer()
                           .AddDbContext<AdventureContext>(OptionsAction);
        }

        private static void OptionsAction(IServiceProvider serviceProvider,
                                          DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabaseConnectionOptions>>().Value;

            var connectionStringBuilder = new DbConnectionStringBuilder()
                                          {
                                              {"Server", $"tcp:{options.Srv},1433"},
                                              {"Initial Catalog", options.Cat},
                                              {"Persist Security Info", "False"},
                                              {"User ID", options.Usr},
                                              {"Password", options.Pwd},
                                              {"MultipleActiveResultSets", "False"},
                                              {"Encrypt", "True"},
                                              {"TrustServerCertificate", "False"},
                                              {"Connection Timeout", "30"},
                                          };
            dbContextOptionsBuilder.UseSqlServer(connectionStringBuilder.ConnectionString)
                                   //.ConfigureWarnings(w =>
                                   //                   {
                                   //                       w.Throw(RelationalEventId.QueryClientEvaluationWarning);
                                   //                   })
                ;
        }
    }
}