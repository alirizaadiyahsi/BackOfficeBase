using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BackOfficeBase.Application.Identity.Dto;
using BackOfficeBase.Utilities.PrimitiveTypes;
using BackOfficeBase.Web.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BackOfficeBase.Tests.IntegrationTests
{
    public class IntegrationTestBase:IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public IntegrationTestBase(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        protected async Task<string> LoginAsAdminUserAndGetTokenAsync()
        {
            return await LoginAndGetTokenAsync("admin", "123qwe");
        }

        protected async Task<string> LoginAndGetTokenAsync(string userNameOrEmail, string password)
        {
            var adminUserLoginViewModel = new LoginInput
            {
                UserNameOrEmail = userNameOrEmail,
                Password = password
            };

            var responseLogin = await _httpClient.PostAsync("/api/login",
                adminUserLoginViewModel.ToStringContent(Encoding.UTF8, "application/json"));

            if (!responseLogin.IsSuccessStatusCode)
            {
                return null;
            }

            var loginResult = await responseLogin.Content.ReadAsAsync<LoginOutput>();
            return loginResult.Token;
        }
    }
}