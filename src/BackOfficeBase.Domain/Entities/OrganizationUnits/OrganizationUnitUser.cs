using System;
using BackOfficeBase.Domain.Entities.Auditing;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Domain.Entities.OrganizationUnits
{
    public class OrganizationUnitUser : CreationAuditedEntity
    {
        public Guid OrganizationUnitId { get; set; }

        public Guid UserId { get; set; }

        public virtual OrganizationUnit OrganizationUnit { get; set; }

        public virtual User User { get; set; }
    }
}