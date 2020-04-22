using AutoMapper;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Tests.Application.Shared.CrudAppService.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;

namespace BackOfficeBase.Tests.Application.Shared.CrudAppService
{
    public class ProductCrudAppService : CrudAppService<Product, ProductDto, ProductDto, CreateProductInput, UpdateProductInput>, IProductCrudAppService
    {
        public ProductCrudAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
