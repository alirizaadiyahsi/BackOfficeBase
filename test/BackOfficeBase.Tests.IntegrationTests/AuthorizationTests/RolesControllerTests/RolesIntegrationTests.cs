using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Tests.IntegrationTests.AuthorizationTests.RolesControllerTests.DataBuilder;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Utilities.PrimitiveTypes;
using BackOfficeBase.Web.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BackOfficeBase.Tests.IntegrationTests.AuthorizationTests.RolesControllerTests
{
    public class RolesIntegrationTests : IClassFixture<RolesWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public RolesIntegrationTests(RolesWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Should_Get_Role()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/roles/{TestDataBuilderForRoles.TestRoleForGet.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            var response = await _httpClient.SendAsync(request);
            var role = await response.Content.ReadAsAsync<RoleOutput>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(TestDataBuilderForRoles.TestRoleForGet.Id, role.Id);
        }

        [Fact]
        public async Task Should_Get_Roles()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/roles");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            var response = await _httpClient.SendAsync(request);
            var roles = await response.Content.ReadAsAsync<PagedListResult<RoleListOutput>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(roles.Items.Any());
        }

        [Fact]
        public async Task Should_Create_Role()
        {
            var input = new CreateRoleInput
            {
                Name = "CreateTestRoleName_" + Guid.NewGuid(),
                SelectedPermissions = new List<string> { "permission1", "permission2" }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/roles");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            request.Content = input.ToStringContent(Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task Should_Update_Role()
        {
            // TODO: insert before update
            var input = new UpdateRoleInput
            {
                Name = "UpdateTestRoleName_" + Guid.NewGuid(),
                SelectedPermissions = new List<string> { "permission1", "permission2" }
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/roles");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            request.Content = input.ToStringContent(Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Delete_Role()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/roles/{TestDataBuilderForRoles.TestRoleForDelete.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            var response = await _httpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
