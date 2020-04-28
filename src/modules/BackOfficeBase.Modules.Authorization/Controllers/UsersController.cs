using System;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Domain.AppConsts.Authorization;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Web.Core.Constants;
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
            if (user == null) return NotFound(Messages.Shared.EntityNotFound);

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
            var appServiceResult = await _userAppService.CreateAsync(input);
            if (!appServiceResult.Success)
            {
                return BadRequest(appServiceResult.Errors);
            }

            return Ok(appServiceResult.Data);
        }

        [HttpPut]
        [Authorize(AppPermissions.Users.Update)]
        public ActionResult<UserOutput> PutUsers(UpdateUserInput input)
        {
            var appServiceResult = _userAppService.Update(input);
            if (!appServiceResult.Success)
            {
                return BadRequest(appServiceResult.Errors);
            }

            return Ok(appServiceResult.Data);
        }

        [HttpDelete]
        [Authorize(AppPermissions.Users.Delete)]
        public async Task<ActionResult<UserOutput>> DeleteUsers(Guid id)
        {
            var appServiceResult = await _userAppService.DeleteAsync(id);
            if (!appServiceResult.Success)
            {
                return BadRequest(appServiceResult.Errors);
            }

            return Ok(appServiceResult.Data);
        }
    }
}
