using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Dto;
using BackOfficeBase.Tests.Application.Shared.Products;
using BackOfficeBase.Tests.Application.Shared.Products.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;
using Xunit;

namespace BackOfficeBase.Tests.Application.Crud
{
    public class CrudAppServiceTests : AppServiceTestBase
    {
        [Fact]
        public async Task Should_Get_Async()
        {
            var dbContext = GetNewInstanceOfDefaultTestDbContext();
            var mapper = GetConfiguredMapper();
            var productCrudAppService = new ProductCrudAppService(dbContext, mapper);

            var result = dbContext.Products.Add(new Product { Name = "Product Name", Code = "product_code" });
            dbContext.SaveChanges();
            var productDto = await productCrudAppService.GetAsync(result.Entity.Id);

            Assert.NotNull(productDto);
            Assert.Equal("product_code", productDto.Code);
        }

        [Fact]
        public async Task Should_Get_List_Async()
        {
            var dbContext = GetNewInstanceOfDefaultTestDbContext();
            var mapper = GetConfiguredMapper();
            var productCrudAppService = new ProductCrudAppService(dbContext, mapper);

            dbContext.Products.Add(new Product { Name = "E Product Name", Code = "e_product_code_for_get_list_with_filter_and_sort_async" });
            dbContext.Products.Add(new Product { Name = "A Product Name", Code = "a_product_code_for_get_list_with_filter_and_sort_async" });
            dbContext.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_1_for_get_list_with_filter_and_sort_async" });
            dbContext.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_2_for_get_list_with_filter_and_sort_async" });
            dbContext.SaveChanges();

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
                    "Name",
                    "Code desc"
                }
            };

            var pagedProductList = await productCrudAppService.GetListAsync(pagedListInput);

            Assert.NotNull(pagedProductList);
            Assert.Equal(4, pagedProductList.TotalCount);
            Assert.Equal("b_product_code_2_for_get_list_with_filter_and_sort_async", pagedProductList.Items.ToArray()[1].Code);
        }

        [Fact]
        public async Task Should_Get_List_With_No_Filter_And_Sort_Async()
        {
            var dbContext = GetNewTestDbContext(Guid.NewGuid().ToString());
            var mapper = GetConfiguredMapper();
            var productCrudAppService = new ProductCrudAppService(dbContext, mapper);

            dbContext.Products.Add(new Product { Name = "E Product Name", Code = "e_product_code_for_get_list_with_no_filter_and_sort_async" });
            dbContext.Products.Add(new Product { Name = "A Product Name", Code = "a_product_code_for_get_list_with_no_filter_and_sort_async" });
            dbContext.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_1_for_get_list_with_no_filter_and_sort_async" });
            dbContext.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_2_for_get_list_with_no_filter_and_sort_async" });
            dbContext.SaveChanges();

            var pagedListInput = new PagedListInput();
            var pagedProductList = await productCrudAppService.GetListAsync(pagedListInput);

            Assert.NotNull(pagedProductList);
            Assert.Equal(4, pagedProductList.TotalCount);
        }

        [Fact]
        public async Task Should_Create_Async()
        {
            var dbContext = GetNewInstanceOfDefaultTestDbContext();
            var mapper = GetConfiguredMapper();
            var productCrudAppService = new ProductCrudAppService(dbContext, mapper);
            var userOutput = await productCrudAppService.CreateAsync(new CreateProductInput
            {
                Code = "create_async_product_code",
                Name = "Create Async Product Name"
            });
            await dbContext.SaveChangesAsync();

            var anotherScopeDbContext = GetNewInstanceOfDefaultTestDbContext();
            var insertedProductDto = await anotherScopeDbContext.Products.FindAsync(userOutput.Id);

            Assert.NotNull(userOutput);
            Assert.NotNull(insertedProductDto);
            Assert.Equal(userOutput.Code, insertedProductDto.Code);
        }

        [Fact]
        public async Task Should_Update_Async()
        {
            var dbContext = GetNewInstanceOfDefaultTestDbContext();
            var mapper = GetConfiguredMapper();
            var productDto = await dbContext.Products.AddAsync(new Product
            {
                Code = "update_product_code",
                Name = "Update Product Name"
            });
            await dbContext.SaveChangesAsync();

            var dbContextForUpdateEntity = GetNewInstanceOfDefaultTestDbContext();
            var productCrudAppService = new ProductCrudAppService(dbContextForUpdateEntity, mapper);
            var userOutput = productCrudAppService.Update(new UpdateProductInput
            {
                Id = productDto.Entity.Id,
                Code = "update_product_code_updated",
                Name = "Update Product Name Updated"
            });
            await dbContextForUpdateEntity.SaveChangesAsync();

            var dbContextForGetEntity = GetNewInstanceOfDefaultTestDbContext();
            var updatedProductDto = await dbContextForGetEntity.Products.FindAsync(productDto.Entity.Id);

            Assert.NotNull(userOutput);
            Assert.NotNull(productDto);
            Assert.NotNull(updatedProductDto);
            Assert.Equal("update_product_code_updated", updatedProductDto.Code);
            Assert.Equal("Update Product Name Updated", updatedProductDto.Name);
        }

        [Fact]
        public async Task Should_Delete_Async()
        {
            var dbContext = GetNewInstanceOfDefaultTestDbContext();
            var mapper = GetConfiguredMapper();
            var productCrudAppService = new ProductCrudAppService(dbContext, mapper);

            var productDto = await dbContext.Products.AddAsync(new Product
            {
                Code = "delete_product_code",
                Name = "Delete Product Name"
            });
            await dbContext.SaveChangesAsync();

            var userOutput = await productCrudAppService.DeleteAsync(productDto.Entity.Id);
            await dbContext.SaveChangesAsync();

            var dbContextForGetEntity = GetNewInstanceOfDefaultTestDbContext();
            var deletedProductDto = await dbContextForGetEntity.Products.FindAsync(productDto.Entity.Id);

            Assert.NotNull(userOutput);
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
