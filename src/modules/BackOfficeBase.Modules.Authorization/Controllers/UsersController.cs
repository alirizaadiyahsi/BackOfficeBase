using System;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authentication;
using BackOfficeBase.Application.Authorization.Users;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Domain.AppConstants;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Web.Core;
using BackOfficeBase.Web.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeBase.Modules.Authorization.Controllers
{
    public class UsersController : ApiControllerBase
    {
        private readonly IUserAppService _userAppService;
        private readonly IAuthenticationAppService _authenticationAppService;

        public UsersController(IUserAppService userAppService, IAuthenticationAppService authenticationAppService)
        {
            _userAppService = userAppService;
            _authenticationAppService = authenticationAppService;
        }

        [HttpGet]
        [Authorize(AppPermissions.Users.Read)]
        public async Task<ActionResult<UserOutput>> GetUsers(Guid id)
        {
            var user = await _userAppService.GetAsync(id);
            if (user == null) return NotFound(UserFriendlyMessages.EntityNotFound);

            return Ok(user);
        }

        [HttpGet]
        [Authorize(AppPermissions.Users.Read)]
        public async Task<ActionResult<IPagedListResult<UserListOutput>>> GetUsers(PagedListInput input)
        {
            var users = await _userAppService.GetListAsync(input);

            return Ok(users);
        }

        [HttpPost]
        [Authorize(AppPermissions.Users.Create)]
        public async Task<ActionResult<UserOutput>> PostUsers(CreateUserInput input)
        {
            var user = await _authenticationAppService.FindUserByEmailAsync(input.Email);
            if (user != null) return Conflict(UserFriendlyMessages.EmailAlreadyExist);

            user = await _authenticationAppService.FindUserByUserNameAsync(input.UserName);
            if (user != null) return Conflict(UserFriendlyMessages.UserNameAlreadyExist);

            var userOutput = await _userAppService.CreateAsync(input);

            return Ok(userOutput);
        }

        [HttpPut]
        [Authorize(AppPermissions.Users.Update)]
        public async Task<ActionResult<UserOutput>> PutUsers(UpdateUserInput input)
        {
            var user = await _authenticationAppService.FindUserByEmailAsync(input.Email);
            if (user != null) return Conflict(UserFriendlyMessages.EmailAlreadyExist);

            user = await _authenticationAppService.FindUserByUserNameAsync(input.UserName);
            if (user != null) return Conflict(UserFriendlyMessages.UserNameAlreadyExist);

            var userOutput = _userAppService.Update(input);

            return Ok(userOutput);
        }

        [HttpDelete]
        [Authorize(AppPermissions.Users.Delete)]
        public async Task<ActionResult<UserOutput>> DeleteUsers(Guid id)
        {
            var userOutput = await _userAppService.DeleteAsync(id);

            return Ok(userOutput);
        }
    }
}
