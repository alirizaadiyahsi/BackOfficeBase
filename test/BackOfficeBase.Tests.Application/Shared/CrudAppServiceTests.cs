using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Tests.Application.Shared.ProductCrudAppService;
using BackOfficeBase.Tests.Application.Shared.ProductCrudAppService.Dto;
using BackOfficeBase.Tests.Shared.DataAccess;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BackOfficeBase.Tests.Application.Shared
{
    public class CrudAppServiceTests : AppServiceTestBase
    {
        private readonly IProductCrudAppService _productCrudAppService;

        public CrudAppServiceTests()
        {
            var mapper = GetConfiguredMapper();
            _productCrudAppService = new ProductCrudAppService.ProductCrudAppService(DbContextTest, mapper);
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
            DbContextTest.Products.Add(new Product { Name = "A Product Name", Code = "a_product_code", CreationTime = DateTime.Now.AddDays(-5) });
            DbContextTest.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_2", CreationTime = DateTime.Now.AddDays(-4) });
            DbContextTest.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_1", CreationTime = DateTime.Now.AddDays(-2) });
            DbContextTest.Products.Add(new Product { Name = "C Product Name", Code = "c_product_code", CreationTime = DateTime.Now.AddDays(-3), IsDeleted = true });
            DbContextTest.Products.Add(new Product { Name = "E Product Name", Code = "e_product_code", CreationTime = DateTime.Now.AddDays(-1) });
            DbContextTest.SaveChanges();

            var pagedListInput = new PagedListInput
            {
                Filters = new List<string>
                {
                    "Name.Contains(\"Product\")",
                    "CreationTime > DateTime.Now.AddDays(-5)",
                    "!IsDeleted"
                },
                Sorts = new List<string>
                {
                    "Name asc",
                    "Code desc"
                }
            };

            var pagedProductList = await _productCrudAppService.GetListAsync(pagedListInput);

            Assert.NotNull(pagedProductList);
            Assert.Equal(3, pagedProductList.TotalCount);
            Assert.Equal(0, pagedProductList.Items.Count(x => x.Code == "c_product_code"));
            Assert.Equal("b_product_code_1", pagedProductList.Items.ToArray()[1].Code);
        }

        [Fact]
        public async Task Should_Create_Async()
        {
            var productDto = await _productCrudAppService.CreateAsync(new CreateProductInput
            {
                Code = "create_async_product_code",
                Name = "Create Async Product Name"
            });
            await DbContextTest.SaveChangesAsync();

            var anotherScopeDbContext = GetNewHostServiceProvider().CreateScope().ServiceProvider.GetRequiredService<BackOfficeBaseDbContextTest>();
            var insertedProductDto = await anotherScopeDbContext.Products.FindAsync(productDto.Id);

            Assert.NotNull(productDto);
            Assert.NotNull(insertedProductDto);
            Assert.Equal(productDto.Code, insertedProductDto.Code);
        }

        [Fact]
        public async Task Should_Update_Async()
        {
            var dbContextForAddEntity = GetNewHostServiceProvider().CreateScope().ServiceProvider.GetRequiredService<BackOfficeBaseDbContextTest>();
            var productDto = await dbContextForAddEntity.Products.AddAsync(new Product
            {
                Code = "update_product_code",
                Name = "Update Product Name"
            });
            await dbContextForAddEntity.SaveChangesAsync();

            _productCrudAppService.Update(new UpdateProductInput
            {
                Id = productDto.Entity.Id,
                Code = "update_product_code_updated",
                Name = "Update Product Name Updated"
            });
            await DbContextTest.SaveChangesAsync();

            var dbContextForGetEntity = GetNewHostServiceProvider().CreateScope().ServiceProvider.GetRequiredService<BackOfficeBaseDbContextTest>();
            var updatedProductDto = await dbContextForGetEntity.Products.FindAsync(productDto.Entity.Id);

            Assert.NotNull(productDto);
            Assert.NotNull(updatedProductDto);
            Assert.Equal("update_product_code_updated", updatedProductDto.Code);
            Assert.Equal("Update Product Name Updated", updatedProductDto.Name);
        }

        [Fact]
        public async Task Should_Delete_Async()
        {
            var dbContextForAddEntity = GetNewHostServiceProvider().CreateScope().ServiceProvider.GetRequiredService<BackOfficeBaseDbContextTest>();
            var productDto = await dbContextForAddEntity.Products.AddAsync(new Product
            {
                Code = "delete_product_code",
                Name = "Delete Product Name"
            });
            await dbContextForAddEntity.SaveChangesAsync();
            
            await _productCrudAppService.DeleteAsync(productDto.Entity.Id);
            await DbContextTest.SaveChangesAsync();

            var dbContextForGetEntity = GetNewHostServiceProvider().CreateScope().ServiceProvider.GetRequiredService<BackOfficeBaseDbContextTest>();
            var deletedProductDto = await dbContextForGetEntity.Products.FindAsync(productDto.Entity.Id);

            Assert.NotNull(productDto);
            Assert.Null(deletedProductDto);
        }

        private static IMapper GetConfiguredMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductDto>();
                cfg.CreateMap<CreateProductInput, Product>();
                cfg.CreateMap<UpdateProductInput, Product>();
            });
            var mapper = mapperConfig.CreateMapper();
            return mapper;
        }
    }
}
