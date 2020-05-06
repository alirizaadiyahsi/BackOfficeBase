using System;
using BackOfficeBase.Application.Dto;

namespace BackOfficeBase.Application.OrganizationUnits.Dto
{
    public class CreateOrganizationUnitInput : EntityDto
    {
        public Guid? ParentId { get; set; }

        public string DisplayName { get; set; }
    }
}