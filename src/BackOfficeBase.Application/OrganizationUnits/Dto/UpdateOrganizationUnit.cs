using System;
using BackOfficeBase.Application.Dto;

namespace BackOfficeBase.Application.OrganizationUnits.Dto
{
    public class UpdateOrganizationUnitInput : EntityDto
    {
        public Guid? ParentId { get; set; }

        public string Name { get; set; }
    }
}