using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackOfficeBase.Tests.Shared
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<BackOfficeBaseDbContext>(options =>
            //{
            //    options.UseInMemoryDatabase("AspNetCoreStarterKit")
            //        .UseLazyLoadingProxies()
            //        .EnableSensitiveDataLogging();
            //});

            //services.AddIdentity<User, Role>()
            //    .AddEntityFrameworkStores<BackOfficeBaseDbContext>()
            //    .AddDefaultTokenProviders();
        }

        public void Configure()
        {

        }
    }
}