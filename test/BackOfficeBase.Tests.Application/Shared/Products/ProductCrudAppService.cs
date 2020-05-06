using AutoMapper;
using BackOfficeBase.Application.Crud;
using BackOfficeBase.Tests.Application.Shared.Products.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.Tests.Application.Shared.Products
{
    public class ProductCrudAppService : CrudAppService<Product, ProductOutput, ProductOutput, CreateProductInput, UpdateProductInput>, IProductCrudAppService
    {
        public ProductCrudAppService(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
