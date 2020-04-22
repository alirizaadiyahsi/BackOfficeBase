using System.Threading.Tasks;
using AutoMapper;
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
    }
}
