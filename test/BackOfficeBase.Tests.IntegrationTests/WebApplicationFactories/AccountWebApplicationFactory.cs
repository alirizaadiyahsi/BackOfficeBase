using System;
using System.Linq;
using BackOfficeBase.DataAccess;
using BackOfficeBase.DataAccess.Helpers;
using BackOfficeBase.Tests.IntegrationTests.AuthenticationTests.DataBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BackOfficeBase.Tests.IntegrationTests.WebApplicationFactories
{
    public class AccountWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<BackOfficeBaseDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<BackOfficeBaseDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForAccountTesting").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    options.EnableSensitiveDataLogging();
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<BackOfficeBaseDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<AccountWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        new DbContextDataBuilderHelper(db).SeedData();
                        new TestDataBuilderForAccount(db).SeedData();
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }
}
