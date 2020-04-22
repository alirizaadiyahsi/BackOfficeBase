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
        private readonly BackOfficeBaseDbContextTest _dbContextTest;

        public CrudAppServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDto>());
            var mapper = mapperConfig.CreateMapper();

            _dbContextTest = GetNewHostServiceProvider().GetRequiredService<BackOfficeBaseDbContextTest>();
            _productCrudAppService = new ProductCrudAppService.ProductCrudAppService(_dbContextTest, mapper);
        }

        [Fact]
        public async Task Should_Get_Async()
        {
            var result = _dbContextTest.Products.Add(new Product { Name = "Product Name", Code = "product_code" });
            _dbContextTest.SaveChanges();

            var productDto = await _productCrudAppService.GetAsync(result.Entity.Id);

            Assert.NotNull(productDto);
            Assert.Equal("product_code", productDto.Code);
        }

        [Fact]
        public async Task Should_Get_List_Async()
        {
            _dbContextTest.Products.Add(new Product { Name = "A Product Name", Code = "a_product_code", CreationTime = DateTime.Now.AddDays(-5) });
            _dbContextTest.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_2", CreationTime = DateTime.Now.AddDays(-4) });
            _dbContextTest.Products.Add(new Product { Name = "B Product Name 1", Code = "b_product_code_1", CreationTime = DateTime.Now.AddDays(-2) });
            _dbContextTest.Products.Add(new Product { Name = "C Product Name", Code = "c_product_code", CreationTime = DateTime.Now.AddDays(-3), IsDeleted = true });
            _dbContextTest.Products.Add(new Product { Name = "E Product Name", Code = "e_product_code", CreationTime = DateTime.Now.AddDays(-1) });
            _dbContextTest.SaveChanges();

            var pagedListInput = new PagedListInput
            {
                Filters = new List<string>
                {
                    "Name.Contains(\"Product\")",
                    $"CreationTime > DateTime.Now.AddDays(-5)",
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
    }
}
