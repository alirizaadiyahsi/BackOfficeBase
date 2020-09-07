using System;
using System.Linq;
using BackOfficeBase.DataAccess;
using BackOfficeBase.DataAccess.Helpers;
using BackOfficeBase.Tests.IntegrationTests.AuthenticationTests.DataBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackOfficeBase.Tests.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly string _connectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;

        public CustomWebApplicationFactory()
        {
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
        }

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

                services
                    .AddEntityFrameworkSqlite()
                    .AddEntityFrameworkProxies()
                    .AddDbContext<BackOfficeBaseDbContext>(options =>
                    {
                        options.UseSqlite(_connection);
                        options.UseInternalServiceProvider(services.BuildServiceProvider());
                        options.UseLazyLoadingProxies();
                    });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<BackOfficeBaseDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                db.Database.EnsureCreated();

                try
                {
                    new DbContextDataBuilderHelper(db).SeedData();
                    // TODO: Call one method instead of calling TestDataBuilderForAccount
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection.Close();
        }
    }
}
