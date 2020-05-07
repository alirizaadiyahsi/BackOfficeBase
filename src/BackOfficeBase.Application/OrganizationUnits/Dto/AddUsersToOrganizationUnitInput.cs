using System;
using System.Collections.Generic;

namespace BackOfficeBase.Application.OrganizationUnits.Dto
{
    public class AddUsersToOrganizationUnitInput
    {
        public Guid OrganizationUnitId { get; set; }

        public IEnumerable<Guid> SelectedUserIds { get; set; }
    }
}