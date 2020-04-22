using BackOfficeBase.Application.Shared.Dto;

namespace BackOfficeBase.Tests.Application.Shared.ProductCrudAppService.Dto
{
    public class ProductDto : AuditedEntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}