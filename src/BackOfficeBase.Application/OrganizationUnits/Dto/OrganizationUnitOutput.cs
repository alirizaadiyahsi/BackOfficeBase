using System.Collections.Generic;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Dto;

namespace BackOfficeBase.Application.OrganizationUnits.Dto
{
    public class OrganizationUnitOutput : EntityDto
    {
        public IEnumerable<RoleOutput> SelectedRoles { get; set; }

        public IEnumerable<UserOutput> SelectedUsers { get; set; }
    }
}