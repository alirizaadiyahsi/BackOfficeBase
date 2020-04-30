namespace BackOfficeBase.Application.Shared.Services.Authorization.Dto
{
    public class ConfirmEmailInput
    {
        public string Email { get; set; }

        public string Token { get; set; }
    }
}