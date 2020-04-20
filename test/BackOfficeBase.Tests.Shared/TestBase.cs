using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace BackOfficeBase.Tests.Shared
{
    public class TestBase
    {
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
