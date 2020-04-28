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
    // TODO: Write test
    public class UsersController : ApiControllerBase
    {
        private readonly IUserAppService _userAppService;

        public UsersController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [HttpGet]
        [Authorize(Permissions.Users.Read)]
        public async Task<ActionResult<UserOutput>> GetUsers(Guid id)
        {
            var user = await _userAppService.GetAsync(id);
            if (user == null) return NotFound(Messages.Shared.EntityNotFound);

            return user;
        }

        [HttpGet]
        [Authorize(Permissions.Users.Read)]
        public async Task<ActionResult<IPagedListResult<UserListOutput>>> GetUsers(PagedListInput input)
        {
            var users = await _userAppService.GetListAsync(input);

            return Ok(users);
        }

        [HttpPost]
        [Authorize(Permissions.Users.Create)]
        public async Task<ActionResult<UserOutput>> PostUsers(CreateUserInput input)
        {
            var appServiceResult = await _userAppService.CreateAsync(input);
            if (!appServiceResult.Success)
            {
                return BadRequest(appServiceResult.Errors);
            }

            return appServiceResult.Data;
        }

        [HttpPut]
        [Authorize(Permissions.Users.Update)]
        public ActionResult<UserOutput> PutUsers(UpdateUserInput input)
        {
            var appServiceResult = _userAppService.Update(input);
            if (!appServiceResult.Success)
            {
                return BadRequest(appServiceResult.Errors);
            }

            return appServiceResult.Data;
        }

        [HttpDelete]
        [Authorize(Permissions.Users.Delete)]
        public async Task<ActionResult<UserOutput>> DeleteUsers(Guid id)
        {
            var appServiceResult = await _userAppService.DeleteAsync(id);
            if (!appServiceResult.Success)
            {
                return BadRequest(appServiceResult.Errors);
            }

            return appServiceResult.Data;
        }
    }
}
