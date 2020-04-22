using AutoMapper;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Tests.Application.Shared.ProductCrudAppService.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;

namespace BackOfficeBase.Tests.Application.Shared.ProductCrudAppService
{
    public class ProductCrudAppService : CrudAppService<Product, ProductDto, ProductDto, CreateProductInput, UpdateProductInput>, IProductCrudAppService
    {
        public ProductCrudAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
