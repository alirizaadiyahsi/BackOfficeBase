using System;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Domain.AppConstants;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Web.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeBase.Modules.Authorization.Controllers
{
    public class UsersController : ApiControllerBase
    {
        private readonly IUserAppService _userAppService;

        public UsersController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
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
            // TODO: Check if user exist and write test case for this
            var userOutput = await _userAppService.CreateAsync(input);

            return Ok(userOutput);
        }

        [HttpPut]
        [Authorize(AppPermissions.Users.Update)]
        public ActionResult<UserOutput> PutUsers(UpdateUserInput input)
        {
            // TODO: Check if user exist and write test case for this
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
