namespace BackOfficeBase.Application.Shared.Services.Authorization.Dto
{
    public class LoginInput
    {
        public string UserNameOrEmail { get; set; }

        public string Password { get; set; }
    }
}
