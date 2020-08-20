using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Identity.Dto;
using BackOfficeBase.DataAccess.Helpers;
using BackOfficeBase.Tests.IntegrationTests.AuthenticationTests.DataBuilder;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Utilities.PrimitiveTypes;
using BackOfficeBase.Web.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BackOfficeBase.Tests.IntegrationTests.AuthenticationTests
{
    public class AccountIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public AccountIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Should_Not_Access_Authorized_Controller()
        {
            var responseUsers = await _httpClient.GetAsync("/api/users");
            Assert.Equal(HttpStatusCode.Unauthorized, responseUsers.StatusCode);
        }

        [Fact]
        public async Task Should_Not_Login_With_Wrong_Credentials()
        {
            var token = await LoginHelper.LoginAndGetTokenAsync("wrongUserName", "wrongPassword", _httpClient);
            Assert.Null(token);
        }

        [Fact]
        public async Task Should_Access_Authorized_Controller()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/users");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginHelper.LoginAsAdminUserAndGetTokenAsync(_httpClient));
            var responseGetUsers = await _httpClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.OK, responseGetUsers.StatusCode);

            var users = await responseGetUsers.Content.ReadAsAsync<PagedListResult<UserListOutput>>();
            Assert.True(users.Items.Any());
        }

        [Fact]
        public async Task Should_Register()
        {
            var registerInput = new RegisterInput
            {
                Email = "TestUserEmail_" + Guid.NewGuid() + "@mail.com",
                UserName = "TestUserName_" + Guid.NewGuid(),
                Password = "aA!121212"
            };

            var responseRegister = await _httpClient.PostAsync("/api/register",
                registerInput.ToStringContent(Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, responseRegister.StatusCode);
        }

        [Fact]
        public async Task Should_Not_Register_With_Existing_User()
        {
            var registerInput = new RegisterInput
            {
                Email = DbContextDataBuilderHelper.AdminUserEmail,
                UserName = DbContextDataBuilderHelper.AdminUserName,
                Password = "aA!121212"
            };

            var responseRegister = await _httpClient.PostAsync("/api/register",
                registerInput.ToStringContent(Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Conflict, responseRegister.StatusCode);
        }

        [Fact]
        public async Task Should_Not_Register_With_Invalid_User()
        {
            var input = new RegisterInput
            {
                Email = new string('*', 300),
                UserName = new string('*', 300),
                Password = "aA!121212"
            };

            var response = await _httpClient.PostAsync("/api/register",
                input.ToStringContent(Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Should_Change_Password()
        {
            var token = await LoginHelper.LoginAndGetTokenAsync(TestDataBuilderForAccount.TestUserForChangePassword.UserName, "123qwe", _httpClient);
            var input = new ChangePasswordInput
            {
                CurrentPassword = "123qwe",
                NewPassword = "aA!121212",
                PasswordRepeat = "aA!121212"
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/changePassword");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = input.ToStringContent(Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Reset_Password()
        {
            var testUser = TestDataBuilderForAccount.TestUserForResetPassword;
            var token = await LoginHelper.LoginAndGetTokenAsync(testUser.UserName, "123qwe", _httpClient);
            var input = new ForgotPasswordInput
            {
                Email = testUser.Email
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/forgotPassword");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = input.ToStringContent(Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(requestMessage);
            var result = await response.Content.ReadAsAsync<ForgotPasswordOutput>();

            var inputResetPassword = new ResetPasswordInput
            {
                UserNameOrEmail = testUser.Email,
                Token = result.ResetToken,
                Password = "aA!123456_123123"
            };

            var requestMessageResetPassword = new HttpRequestMessage(HttpMethod.Post, "/api/resetPassword");
            requestMessageResetPassword.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessageResetPassword.Content = inputResetPassword.ToStringContent(Encoding.UTF8, "application/json");

            var responseResetPassword = await _httpClient.SendAsync(requestMessageResetPassword);
            Assert.Equal(HttpStatusCode.OK, responseResetPassword.StatusCode);
        }
    }
}
