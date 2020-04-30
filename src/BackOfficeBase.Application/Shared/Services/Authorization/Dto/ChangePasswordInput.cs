namespace BackOfficeBase.Application.Shared.Services.Authorization.Dto
{
    public class ChangePasswordInput
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string PasswordRepeat { get; set; }
    }
}
