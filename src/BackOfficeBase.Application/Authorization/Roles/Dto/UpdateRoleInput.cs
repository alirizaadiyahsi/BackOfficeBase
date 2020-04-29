using System.Collections.Generic;
using BackOfficeBase.Application.Shared.Dto;

namespace BackOfficeBase.Application.Authorization.Roles.Dto
{
    public class UpdateRoleInput: EntityDto
    {
        public string Name { get; set; }

        public IEnumerable<string> SelectedPermissions { get; set; }

        public IEnumerable<string> AllPermissions{ get; set; }
    }
}