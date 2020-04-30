using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.Application.Shared.Services.Crud;
using BackOfficeBase.Tests.Application.Shared.Products.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;

namespace BackOfficeBase.Tests.Application.Shared.Products
{
    public interface IProductCrudAppService:ICrudAppService<Product, ProductOutput, ProductOutput, CreateProductInput, UpdateProductInput>
    {
    }
}