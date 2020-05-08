using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BackOfficeBase.Application.Identity
{
    // TODO: Make this app service as identityManager and use entities instead of DTOs
    // Because data is losing if it map entity to DTO and re-map again to entity 
    public class IdentityAppService : IIdentityAppService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public IdentityAppService(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<bool> CheckPasswordAsync(UserOutput userOutput, string password)
        {
            var user = _mapper.Map<User>(userOutput);

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<UserOutput> FindUserByUserNameOrEmailAsync(string userNameOrEmail)
        {
            var user = await _userManager.FindByNameAsync(userNameOrEmail) ??
                       await _userManager.FindByEmailAsync(userNameOrEmail);

            return user == null ? null : MapUserToUserOutput(user);
        }

        public async Task<UserOutput> FindUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user == null ? null : MapUserToUserOutput(user);
        }

        public async Task<UserOutput> FindUserByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user == null ? null : MapUserToUserOutput(user);
        }

        public async Task<IdentityResult> CreateUserAsync(UserOutput userOutput, string password)
        {
            var user = _mapper.Map<User>(userOutput);

            return await _userManager.CreateAsync(user, password);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(UserOutput userOutput)
        {
            var user = _mapper.Map<User>(userOutput);

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(UserOutput userOutput, string token)
        {
            var user = _mapper.Map<User>(userOutput);

            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<IdentityResult> ChangePasswordAsync(UserOutput userOutput, string currentPassword, string newPassword)
        {
            var user = _mapper.Map<User>(userOutput);

            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(UserOutput userOutput)
        {
            var user = _mapper.Map<User>(userOutput);

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(UserOutput userOutput, string token, string password)
        {
            var user = _mapper.Map<User>(userOutput);

            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<RoleOutput> FindRoleByNameAsync(string name)
        {
            var roleOutput = _mapper.Map<RoleOutput>(await _roleManager.FindByNameAsync(name));
            roleOutput.AllPermissions = AppPermissions.GetAll();

            return roleOutput;
        }

        public IEnumerable<RoleOutput> GetRolesByUserName(string userName)
        {
            var roles = _roleManager.Roles
                .SelectMany(x => x.UserRoles)
                .Where(x => x.User.UserName == userName)
                .Select(x => x.Role);

            return _mapper.Map<IEnumerable<RoleOutput>>(roles);
        }

        private UserOutput MapUserToUserOutput(User user)
        {
            var userOutput = _mapper.Map<UserOutput>(user);
            userOutput.AllRoles = _mapper.Map<IEnumerable<RoleOutput>>(_roleManager.Roles);
            userOutput.AllPermissions = AppPermissions.GetAll();

            return userOutput;
        }
    }
}