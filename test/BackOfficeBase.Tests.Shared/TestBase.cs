using System;
using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Tests.Shared.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackOfficeBase.Tests.Shared
{
    public class TestBase
    {
        protected readonly TestBackOfficeBaseDbContext DefaultTestDbContext;

        public TestBase()
        {
            DefaultTestDbContext = GetDefaultTestDbContext();
        }

        /// <summary>
        /// This method can be used for new scoped (same database) of default dbContext
        /// </summary>
        /// <returns>New scoped instance (same database) of default dbContext</returns>
        protected TestBackOfficeBaseDbContext GetDefaultTestDbContext()
        {
            var provider = GetNewHostServiceProvider().CreateScope().ServiceProvider;
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new[]
                {
                    new ClaimsIdentity(new[] {new Claim("Id", Guid.NewGuid().ToString())})
                })
            };

            return provider.GetRequiredService<TestBackOfficeBaseDbContext>();
        }

        /// <summary>
        /// This method can be used for create a new dbContext (different database) with different name in run-time.
        /// It is useful when you want to work on an empty database.
        /// </summary>
        /// <param name="dbContextName"></param>
        /// <returns>Creates new dbContext (new database) with different name</returns>
        protected TestBackOfficeBaseDbContext GetNewTestDbContext(string dbContextName)
        {
            var provider = GetNewHostServiceProvider().CreateScope().ServiceProvider;
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new[]
                {
                    new ClaimsIdentity(new[] {new Claim("Id", Guid.NewGuid().ToString())})
                })
            };

            var dbContextOptionBuilder = new DbContextOptionsBuilder();
            dbContextOptionBuilder.UseInMemoryDatabase(dbContextName)
                .UseLazyLoadingProxies()
                .EnableSensitiveDataLogging();

            return new TestBackOfficeBaseDbContext(dbContextOptionBuilder.Options, httpContextAccessor);
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

        public static void AddUserToRole(User testUser, Role testRole)
        {
            var testUserRole = new UserRole
            {
                User = testUser,
                Role = testRole
            };

            testUser.UserRoles.Add(testUserRole);
            testRole.UserRoles.Add(testUserRole);
        }

        public Role GetTestRole(string roleName = "TestRoleName")
        {
            var testRole = new Role
            {
                Name = roleName
            };

            return testRole;
        }

        public User GetTestUser(string userName = "TestUserName", string email = "testuser@mail.com")
        {
            var testUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = email,
                IsDeleted = false,
                EmailConfirmed = true,
                NormalizedEmail = email.ToUpper(CultureInfo.GetCultureInfo("en-US")),
                NormalizedUserName = userName.ToUpper(CultureInfo.GetCultureInfo("en-US")),
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            return testUser;
        }

        public UserOutput GetTestUserOutput(string userName = "TestUserName", string email = "testuser@mail.com")
        {
            var mapper = GetNewHostServiceProvider().GetRequiredService<IMapper>();

            return mapper.Map<UserOutput>(GetTestUser(userName, email));
        }
    }
}
