﻿using System;
using System.Collections.Generic;

namespace BackOfficeBase.Application.OrganizationUnits.Dto
{
    public class AddOrRemoveUsersToOrganizationUnitInput
    {
        public Guid OrganizationUnitId { get; set; }

        public IEnumerable<Guid> SelectedUserIds { get; set; }
    }
}