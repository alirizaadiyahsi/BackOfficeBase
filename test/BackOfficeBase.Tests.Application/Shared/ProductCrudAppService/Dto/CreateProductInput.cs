using BackOfficeBase.Domain.Entities.Auditing;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Tests.Application.Shared.ProductCrudAppService.Dto
{
    public class CreateProductInput:FullAuditedEntity<User>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}