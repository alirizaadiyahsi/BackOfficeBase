using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using Microsoft.AspNetCore.Identity;

namespace BackOfficeBase.Application.Shared.Services.Authorization
{
    public interface IAuthorizationAppService
    {
        Task<bool> CheckPasswordAsync(UserOutput userOutput, string password);
        Task<UserOutput> FindUserByUserNameOrEmailAsync(string userNameOrEmail);
        Task<UserOutput> FindUserByEmailAsync(string email);
        Task<UserOutput> FindUserByUserNameAsync(string userName);
        Task<IdentityResult> CreateUserAsync(UserOutput userOutput, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(UserOutput userOutput);
        Task<IdentityResult> ConfirmEmailAsync(UserOutput userOutput, string token);
        Task<IdentityResult> ChangePasswordAsync(UserOutput userOutput, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(UserOutput userOutput);
        Task<IdentityResult> ResetPasswordAsync(UserOutput userOutput, string token, string password);
        Task<RoleOutput> FindRoleByNameAsync(string name);
    }
}
