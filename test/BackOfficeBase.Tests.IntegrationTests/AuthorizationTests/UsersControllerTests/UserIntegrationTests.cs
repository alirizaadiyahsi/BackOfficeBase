using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Tests.IntegrationTests.AuthorizationTests.UsersControllerTests.DataBuilder;
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
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{TestDataBuilderForUsers.TestUserForGet.Id}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            var responseGetUser = await _httpClient.SendAsync(requestMessage);
            var user = await responseGetUser.Content.ReadAsAsync<UserOutput>();
            
            Assert.Equal(HttpStatusCode.OK, responseGetUser.StatusCode);
            Assert.Equal(TestDataBuilderForUsers.TestUserForGet.Id, user.Id);
        }
    }
}
