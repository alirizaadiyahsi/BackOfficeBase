using System;
using System.Threading.Tasks;
using BackOfficeBase.Application.Dto;
using BackOfficeBase.Application.OrganizationUnits;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Web.Core;
using BackOfficeBase.Web.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeBase.Modules.Core.Controllers
{
    // TODO: Write tests
    public class OrganizationUnitsController : ApiControllerBase
    {
        private readonly IOrganizationUnitAppService _organizationUnitAppService;

        public OrganizationUnitsController(IOrganizationUnitAppService organizationUnitAppService)
        {
            _organizationUnitAppService = organizationUnitAppService;
        }

        [HttpGet("{id}")]
        [Authorize(AppPermissions.OrganizationUnits.Read)]
        public async Task<ActionResult<OrganizationUnitOutput>> GetOrganizationUnits(Guid id)
        {
            var organizationUnit = await _organizationUnitAppService.GetAsync(id);
            if (organizationUnit == null) return NotFound(UserFriendlyMessages.EntityNotFound);

            return Ok(organizationUnit);
        }

        [HttpGet]
        [Authorize(AppPermissions.OrganizationUnits.Read)]
        public async Task<ActionResult<IPagedListResult<OrganizationUnitListOutput>>> GetOrganizationUnits([FromQuery]PagedListInput input)
        {
            var organizationUnits = await _organizationUnitAppService.GetListAsync(input);

            return Ok(organizationUnits);
        }

        [HttpPost]
        [Authorize(AppPermissions.OrganizationUnits.Create)]
        public async Task<ActionResult<OrganizationUnitOutput>> PostOrganizationUnits([FromBody]CreateOrganizationUnitInput input)
        {
            var organizationUnitOutput = await _organizationUnitAppService.CreateAsync(input);

            return Ok(organizationUnitOutput);
        }

        [HttpPut]
        [Authorize(AppPermissions.OrganizationUnits.Update)]
        public async Task<ActionResult<OrganizationUnitOutput>> PutOrganizationUnits([FromBody]UpdateOrganizationUnitInput input)
        {
            var organizationUnitOutput = _organizationUnitAppService.Update(input);

            return Ok(organizationUnitOutput);
        }

        [HttpDelete]
        [Authorize(AppPermissions.OrganizationUnits.Delete)]
        public async Task<ActionResult<OrganizationUnitOutput>> DeleteOrganizationUnits(Guid id)
        {
            var organizationUnitOutput = await _organizationUnitAppService.DeleteAsync(id);

            return Ok(organizationUnitOutput);
        }

        [HttpPost("/api/[action]")]
        [Authorize(AppPermissions.OrganizationUnits.AddUsersToOrganizationUnit)]
        public async Task<ActionResult> AddUsersToOrganizationUnit([FromBody]AddOrRemoveUsersToOrganizationUnitInput input)
        {
            await _organizationUnitAppService.AddUsersToOrganizationUnitAsync(input);

            return Ok();
        }

        [HttpDelete("/api/[action]")]
        [Authorize(AppPermissions.OrganizationUnits.RemoveUsersToOrganizationUnit)]
        public async Task<ActionResult> RemoveUsersFromOrganizationUnit([FromBody]AddOrRemoveUsersToOrganizationUnitInput input)
        {
            _organizationUnitAppService.RemoveUsersFromOrganizationUnit(input);

            return Ok();
        }

        [HttpPost("/api/[action]")]
        [Authorize(AppPermissions.OrganizationUnits.AddRolesToOrganizationUnit)]
        public async Task<ActionResult> AddRolesToOrganizationUnit([FromBody]AddOrRemoveRolesToOrganizationUnitInput input)
        {
            await _organizationUnitAppService.AddRolesToOrganizationUnitAsync(input);

            return Ok();
        }

        [HttpDelete("/api/[action]")]
        [Authorize(AppPermissions.OrganizationUnits.RemoveRolesToOrganizationUnit)]
        public async Task<ActionResult> RemoveRolesFromOrganizationUnit([FromBody]AddOrRemoveRolesToOrganizationUnitInput input)
        {
            _organizationUnitAppService.RemoveRolesFromOrganizationUnit(input);

            return Ok();
        }
    }
}
