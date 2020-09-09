using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Identity.Dto;
using BackOfficeBase.DataAccess.Helpers;
using BackOfficeBase.Tests.IntegrationTests.AuthorizationTests.UsersControllerTests.DataBuilder;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Utilities.PrimitiveTypes;
using BackOfficeBase.Web.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BackOfficeBase.Tests.IntegrationTests.AuthorizationTests.UsersControllerTests
{
    public class UserIntegrationTests : IClassFixture<UsersWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public UserIntegrationTests(UsersWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Should_Get_User()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{TestDataBuilderForUsers.TestUserForGet.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            var response = await _httpClient.SendAsync(request);
            var user = await response.Content.ReadAsAsync<UserOutput>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(TestDataBuilderForUsers.TestUserForGet.Id, user.Id);
        }

        [Fact]
        public async Task Should_Get_Users()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            var response = await _httpClient.SendAsync(request);
            var users = await response.Content.ReadAsAsync<PagedListResult<UserListOutput>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(users.Items.Any());
        }

        [Fact]
        public async Task Should_Create_User()
        {
            var input = new CreateUserInput
            {
                Email = "CreateTestUserEmail_" + Guid.NewGuid() + "@mail.com",
                UserName = "CreateTestUserName_" + Guid.NewGuid(),
                SelectedPermissions = new List<string> { "permission1", "permission2" }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            request.Content = input.ToStringContent(Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task Should_Update_User()
        {
            var input = new UpdateUserInput
            {
                Email = TestDataBuilderForUsers.TestUserForUpdate.Email,
                UserName = TestDataBuilderForUsers.TestUserForUpdate.UserName,
                SelectedPermissions = new List<string> { "permission1", "permission2" },
                FirstName = "Update FirstName",
                LastName = "Updated LastName"
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            request.Content = input.ToStringContent(Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Delete_User()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/{TestDataBuilderForUsers.TestUserForDelete.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            var response = await _httpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
