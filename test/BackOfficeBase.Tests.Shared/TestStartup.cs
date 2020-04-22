using BackOfficeBase.Application;
using BackOfficeBase.Tests.Shared.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackOfficeBase.Tests.Shared
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BackOfficeBaseDbContextTest>(options =>
            {
                options.UseInMemoryDatabase("BackOfficeBaseDbContextTest")
                    .UseLazyLoadingProxies()
                    .EnableSensitiveDataLogging();
            });

            services.ConfigureNucleusApplication();
            services.AddHttpContextAccessor();
        }

        public void Configure()
        {

        }
    }
}