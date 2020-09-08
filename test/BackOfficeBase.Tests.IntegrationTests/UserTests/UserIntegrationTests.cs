using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Tests.IntegrationTests.UserTests.DataBuilder;
using BackOfficeBase.Web.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BackOfficeBase.Tests.IntegrationTests.UserTests
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
