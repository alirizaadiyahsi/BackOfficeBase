using System;
using BackOfficeBase.Domain.Entities.Auditing;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Domain.Entities.OrganizationUnits
{
    public class OrganizationUnitRole : CreationAuditedEntity
    {
        public Guid OrganizationUnitId { get; set; }

        public Guid RoleId { get; set; }

        public virtual OrganizationUnit OrganizationUnit { get; set; }

        public virtual Role Role { get; set; }
    }
}