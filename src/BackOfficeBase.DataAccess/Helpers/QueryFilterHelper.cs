using BackOfficeBase.DataAccess.Extensions;
using BackOfficeBase.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.DataAccess.Helpers
{
    public class QueryFilterHelper
    {
        public static void AddQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                    modelBuilder.Entity(entityType.ClrType).AddQueryFilter<ISoftDelete>(e => e.IsDeleted == false);
            }
        }
    }
}
