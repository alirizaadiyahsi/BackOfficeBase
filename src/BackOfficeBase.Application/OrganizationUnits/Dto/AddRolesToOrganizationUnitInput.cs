using System;
using System.Collections.Generic;

namespace BackOfficeBase.Application.OrganizationUnits.Dto
{
    public class AddOrRemoveRolesToOrganizationUnitInput
    {
        public Guid OrganizationUnitId { get; set; }

        public IEnumerable<Guid> SelectedRoleIds { get; set; }
    }
}