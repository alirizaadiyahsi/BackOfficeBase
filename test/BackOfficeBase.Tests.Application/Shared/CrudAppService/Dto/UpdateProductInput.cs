using BackOfficeBase.Application.Shared.Dto;

namespace BackOfficeBase.Tests.Application.Shared.CrudAppService.Dto
{
    public class UpdateProductInput : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}