using AutoMapper;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.Application.Shared.Services.Crud;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Tests.Application.Shared.Products.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;

namespace BackOfficeBase.Tests.Application.Shared.Products
{
    public class ProductCrudAppService : CrudAppService<Product, ProductOutput, ProductOutput, CreateProductInput, UpdateProductInput>, IProductCrudAppService
    {
        public ProductCrudAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
