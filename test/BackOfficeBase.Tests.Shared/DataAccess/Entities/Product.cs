using BackOfficeBase.Domain.Entities;

namespace BackOfficeBase.Tests.Shared.DataAccess.Entities
{
    public class Product : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}