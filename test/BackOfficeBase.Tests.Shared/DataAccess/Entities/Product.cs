using BackOfficeBase.Domain.Entities.Auditing;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Tests.Shared.DataAccess.Entities
{
    public class Product : FullAuditedEntity<User>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}