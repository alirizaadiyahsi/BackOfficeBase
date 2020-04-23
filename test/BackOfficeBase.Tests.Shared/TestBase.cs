using System;
using System.Collections.Generic;
using System.Security.Claims;
using BackOfficeBase.Tests.Shared.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BackOfficeBase.Tests.Shared
{
    public class TestBase
    {
        protected BackOfficeBaseDbContextTest GetDbContextTest()
        {
            var provider = GetNewHostServiceProvider().CreateScope().ServiceProvider;
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new List<ClaimsIdentity>
                {
                    new ClaimsIdentity(new List<Claim> {new Claim("Id", Guid.NewGuid().ToString())})
                })
            };

            return provider.GetRequiredService<BackOfficeBaseDbContextTest>();
        }

        protected IServiceProvider GetNewHostServiceProvider()
        {
            return GetTestServer().Services;
        }

        protected TestServer GetTestServer()
        {
            return new TestServer(
                new WebHostBuilder()
                    .UseStartup<TestStartup>()
                    .UseEnvironment("Test")
            );
        }
    }
}
