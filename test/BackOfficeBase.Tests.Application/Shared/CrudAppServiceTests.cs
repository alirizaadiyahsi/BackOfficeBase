using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Tests.Application.Shared.Products;
using BackOfficeBase.Tests.Application.Shared.Products.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;
using Xunit;

namespace BackOfficeBase.Tests.Application.Shared
{
    public class CrudAppServiceTests : AppServiceTestBase
    {
        private readonly IProductCrudAppService _productCrudAppService;

        public CrudAppServiceTests()
        {
            var mapper = GetConfiguredMapper();
            _productCrudAppService = new ProductCrudAppService(DbContextTest, mapper);
        }

        [Fact]
        public async Task Should_Get_Async()
        {
            var result = DbContextTest.Products.Add(new Product { Name = "Product Name", Code = "product_code" });
            DbContextTest.SaveChanges();

            var productDto = await _productCrudAppService.GetAsync(result.Entity.Id);

            Assert.NotNull(productDto);
            Assert.Equal("product_code", productDto.Code);
        }

        [Fact]
        public async Task Should_Get_List_Async()
        {
            DbContextTest.Products.Add(new Product { Name = "E Product Name", Code = "e_product_code_for_get_list_with_filter_and_sort_async" });
            DbContextTest.Products.Add(new Product { Name = "A Product Name", Code = "a_product_code_for_get_list_with_filter_and_sort_async" });
            DbContextTest.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_1_for_get_list_with_filter_and_sort_async" });
            DbContextTest.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_2_for_get_list_with_filter_and_sort_async" });
            DbContextTest.SaveChanges();

            var pagedListInput = new PagedListInput
            {
                Filters = new List<string>
                {
                    "Name.Contains(\"Product\")",
                    "CreationTime > DateTime.Now.AddMinutes(-1)",
                    "Code.Contains(\"for_get_list_with_filter_and_sort_async\")"
                },
                Sorts = new List<string>
                {
                    "Name asc",
                    "Code desc"
                }
            };

            var pagedProductList = await _productCrudAppService.GetListAsync(pagedListInput);

            Assert.NotNull(pagedProductList);
            Assert.Equal(4, pagedProductList.TotalCount);
            Assert.Equal("b_product_code_2_for_get_list_with_filter_and_sort_async", pagedProductList.Items.ToArray()[1].Code);
        }

        [Fact]
        public async Task Should_Get_List_With_No_Filter_And_Sort_Async()
        {
            DbContextTest.Products.Add(new Product { Name = "E Product Name", Code = "e_product_code_for_get_list_with_no_filter_and_sort_async" });
            DbContextTest.Products.Add(new Product { Name = "A Product Name", Code = "a_product_code_for_get_list_with_no_filter_and_sort_async" });
            DbContextTest.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_1_for_get_list_with_no_filter_and_sort_async" });
            DbContextTest.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_2_for_get_list_with_no_filter_and_sort_async" });
            DbContextTest.SaveChanges();

            var pagedListInput = new PagedListInput();

            var pagedProductList = await _productCrudAppService.GetListAsync(pagedListInput);

            Assert.NotNull(pagedProductList);
            Assert.Equal(4, pagedProductList.TotalCount);
        }

        [Fact]
        public async Task Should_Create_Async()
        {
            var appServiceResult = await _productCrudAppService.CreateAsync(new CreateProductInput
            {
                Code = "create_async_product_code",
                Name = "Create Async Product Name"
            });
            await DbContextTest.SaveChangesAsync();

            var anotherScopeDbContext = GetDbContextTest();
            var insertedProductDto = await anotherScopeDbContext.Products.FindAsync(appServiceResult.Data.Id);

            Assert.True(appServiceResult.Success);
            Assert.NotNull(appServiceResult.Data);
            Assert.NotNull(insertedProductDto);
            Assert.Equal(appServiceResult.Data.Code, insertedProductDto.Code);
        }

        [Fact]
        public async Task Should_Update_Async()
        {
            var dbContextForAddEntity = GetDbContextTest();
            var productDto = await dbContextForAddEntity.Products.AddAsync(new Product
            {
                Code = "update_product_code",
                Name = "Update Product Name"
            });
            await dbContextForAddEntity.SaveChangesAsync();

            var appServiceResult = _productCrudAppService.Update(new UpdateProductInput
            {
                Id = productDto.Entity.Id,
                Code = "update_product_code_updated",
                Name = "Update Product Name Updated"
            });
            await DbContextTest.SaveChangesAsync();

            var dbContextForGetEntity = GetDbContextTest();
            var updatedProductDto = await dbContextForGetEntity.Products.FindAsync(productDto.Entity.Id);

            Assert.True(appServiceResult.Success);
            Assert.NotNull(productDto);
            Assert.NotNull(updatedProductDto);
            Assert.Equal("update_product_code_updated", updatedProductDto.Code);
            Assert.Equal("Update Product Name Updated", updatedProductDto.Name);
        }

        [Fact]
        public async Task Should_Delete_Async()
        {
            var dbContextForAddEntity = GetDbContextTest();
            var productDto = await dbContextForAddEntity.Products.AddAsync(new Product
            {
                Code = "delete_product_code",
                Name = "Delete Product Name"
            });
            await dbContextForAddEntity.SaveChangesAsync();

            var appServiceResult = await _productCrudAppService.DeleteAsync(productDto.Entity.Id);
            await DbContextTest.SaveChangesAsync();

            var dbContextForGetEntity = GetDbContextTest();
            var deletedProductDto = await dbContextForGetEntity.Products.FindAsync(productDto.Entity.Id);

            Assert.True(appServiceResult.Success);
            Assert.Null(deletedProductDto);
        }

        private static IMapper GetConfiguredMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductOutput>();
                cfg.CreateMap<CreateProductInput, Product>();
                cfg.CreateMap<UpdateProductInput, Product>();
            });
            var mapper = mapperConfig.CreateMapper();
            return mapper;
        }
    }
}
