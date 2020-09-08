using System;
using System.Linq;
using BackOfficeBase.DataAccess;
using BackOfficeBase.DataAccess.Helpers;
using BackOfficeBase.Domain.AppConstants.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackOfficeBase.Tests.IntegrationTests.AccountTests.Configuration
{
    public class AccountWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder() =>
            base.CreateHostBuilder()
                .ConfigureHostConfiguration(
                    config => config.AddEnvironmentVariables("Test"));

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

                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                services.AddDbContext<BackOfficeBaseDbContext>(options =>
                    options.UseNpgsql(string.Format(configuration.GetConnectionString(AppConfig.DefaultTestConnection), "Account"))
                        .UseLazyLoadingProxies());

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<BackOfficeBaseDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<AccountWebApplicationFactory<TStartup>>>();

                db.Database.EnsureDeleted();
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
            });
        }
    }
}
