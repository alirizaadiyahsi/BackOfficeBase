﻿using BackOfficeBase.Application.Shared.Dto;

namespace BackOfficeBase.Tests.Application.Shared.ProductCrudAppService.Dto
{
    public class CreateProductInput : FullAuditedEntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}