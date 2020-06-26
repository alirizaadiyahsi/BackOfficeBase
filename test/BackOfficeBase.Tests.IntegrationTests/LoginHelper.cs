using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BackOfficeBase.Application.Identity.Dto;
using BackOfficeBase.Utilities.PrimitiveTypes;

namespace BackOfficeBase.Tests.IntegrationTests
{
    public class LoginHelper
    {
        public static async Task<string> LoginAsAdminUserAndGetTokenAsync(HttpClient httpClient)
        {
            return await LoginAndGetTokenAsync("admin", "123qwe", httpClient);
        }

        public static async Task<string> LoginAndGetTokenAsync(string userNameOrEmail, string password, HttpClient httpClient)
        {
            var adminUserLoginViewModel = new LoginInput
            {
                UserNameOrEmail = userNameOrEmail,
                Password = password
            };

            var responseLogin = await httpClient.PostAsync("/api/login",
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