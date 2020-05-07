using System;
using System.Linq;
using System.Threading.Tasks;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;
using Xunit;

namespace BackOfficeBase.Tests.DataAccess
{
    public class GlobalQueryFiltersTests : DataAccessTestBase
    {
        [Fact]
        public async Task Should_Add_Creation_Auditing_Properties()
        {
            var result = await DefaultTestDbContext.Products.AddAsync(new Product { Name = "Product Name", Code = "product_code" });
            await DefaultTestDbContext.SaveChangesAsync();

            Assert.NotNull(result.Entity.CreatorUserId);
            Assert.NotEqual(Guid.Empty, result.Entity.CreatorUserId);
            Assert.Equal(DateTime.Today.ToShortDateString(), result.Entity.CreationTime.ToShortDateString());
        }

        [Fact]
        public async Task Should_Add_Modification_Auditing_Properties()
        {
            var result = await DefaultTestDbContext.Products.AddAsync(new Product { Name = "Product Name", Code = "product_code" });
            await DefaultTestDbContext.SaveChangesAsync();

            DefaultTestDbContext.Products.Update(result.Entity);
            await DefaultTestDbContext.SaveChangesAsync();

            var dbContextToGetUpdatedEntity = GetDefaultTestDbContext();
            var updatedEntity = await dbContextToGetUpdatedEntity.Products.FindAsync(result.Entity.Id);

            Assert.NotNull(updatedEntity.ModifierUserId);
            Assert.NotEqual(Guid.Empty, updatedEntity.ModifierUserId);
            Assert.NotNull(updatedEntity.ModificationTime);
            Assert.Equal(DateTime.Today.ToShortDateString(), updatedEntity.ModificationTime.Value.ToShortDateString());
        }

        [Fact]
        public async Task Should_Add_Deletion_Auditing_Properties()
        {
            var result = await DefaultTestDbContext.Products.AddAsync(new Product { Name = "Product Name", Code = "product_code" });
            await DefaultTestDbContext.SaveChangesAsync();

            var deletedEntity = DefaultTestDbContext.Products.Remove(result.Entity);
            await DefaultTestDbContext.SaveChangesAsync();

            Assert.NotNull(deletedEntity.Entity.DeleterUserId);
            Assert.NotEqual(Guid.Empty, deletedEntity.Entity.DeleterUserId);
            Assert.NotNull(deletedEntity.Entity.DeletionTime);
            Assert.Equal(DateTime.Today.ToShortDateString(), deletedEntity.Entity.DeletionTime.Value.ToShortDateString());
        }

        [Fact]
        public async Task Should_Add_Soft_Delete_Query_Filter()
        {
            await DefaultTestDbContext.Products.AddAsync(new Product { Name = "Product Name 1", Code = "soft_deleted_product_code_1" });
            await DefaultTestDbContext.Products.AddAsync(new Product { Name = "Product Name 2", Code = "soft_deleted_product_code_2", IsDeleted = true });
            await DefaultTestDbContext.SaveChangesAsync();

            var productList = DefaultTestDbContext.Products.Where(x => x.Code.Contains("soft_deleted_product_code"));

            Assert.NotNull(productList);
            Assert.Equal(1, productList.Count());
        }
    }
}
