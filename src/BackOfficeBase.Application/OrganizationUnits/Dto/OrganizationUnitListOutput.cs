using System;
using System.Collections.Generic;
using BackOfficeBase.Application.Shared.Dto;

namespace BackOfficeBase.Application.OrganizationUnits.Dto
{
    public class OrganizationUnitListOutput : EntityDto
    {
        public Guid? ParentId { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public virtual OrganizationUnitOutput Parent { get; set; }

        public virtual ICollection<OrganizationUnitOutput> Children { get; set; }
    }
}