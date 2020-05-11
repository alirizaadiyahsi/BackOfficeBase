using System;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Roles;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Dto;
using BackOfficeBase.Application.Identity;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Web.Core;
using BackOfficeBase.Web.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeBase.Modules.Authorization.Controllers
{
    public class RolesController: ApiControllerBase
    {
        private readonly IRoleAppService _roleAppService;
        private readonly IIdentityAppService _identityAppService;

        public RolesController(IRoleAppService roleAppService, IIdentityAppService identityAppService)
        {
            _roleAppService = roleAppService;
            _identityAppService = identityAppService;
        }

        [HttpGet("{id}")]
        [Authorize(AppPermissions.Roles.Read)]
        public async Task<ActionResult<RoleOutput>> GetRoles(Guid id)
        {
            var role = await _roleAppService.GetAsync(id);
            if (role == null) return NotFound(UserFriendlyMessages.EntityNotFound);

            return Ok(role);
        }

        [HttpGet]
        [Authorize(AppPermissions.Roles.Read)]
        public async Task<ActionResult<IPagedListResult<RoleListOutput>>> GetRoles([FromQuery]PagedListInput input)
        {
            var roles = await _roleAppService.GetListAsync(input);

            return Ok(roles);
        }

        [HttpPost]
        [Authorize(AppPermissions.Roles.Create)]
        public async Task<ActionResult<RoleOutput>> PostRoles([FromBody]CreateRoleInput input)
        {
            var role = await _identityAppService.FindRoleByNameAsync(input.Name);
            if (role != null) return Conflict(UserFriendlyMessages.RoleNameAlreadyExist);

            var roleOutput = await _roleAppService.CreateAsync(input);

            return Ok(roleOutput);
        }

        [HttpPut]
        [Authorize(AppPermissions.Roles.Update)]
        public async Task<ActionResult<RoleOutput>> PutRoles([FromBody]UpdateRoleInput input)
        {
            var role = await _identityAppService.FindRoleByNameAsync(input.Name);
            if (role != null) return Conflict(UserFriendlyMessages.RoleNameAlreadyExist);

            var roleOutput = _roleAppService.Update(input);

            return Ok(roleOutput);
        }

        [HttpDelete]
        [Authorize(AppPermissions.Roles.Delete)]
        public async Task<ActionResult<RoleOutput>> DeleteRoles(Guid id)
        {
            var roleOutput = await _roleAppService.DeleteAsync(id);

            return Ok(roleOutput);
        }
    }
}
