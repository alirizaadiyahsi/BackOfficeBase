﻿using BackOfficeBase.Application.Dto;

namespace BackOfficeBase.Tests.Application.Shared.Products.Dto
{
    public class CreateProductInput : AuditedEntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}