using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.Tests.Application.Shared.ProductCrudAppService.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;

namespace BackOfficeBase.Tests.Application.Shared.ProductCrudAppService
{
    public interface IProductCrudAppService:ICrudAppService<Product, ProductOutput, ProductOutput, CreateProductInput, UpdateProductInput>
    {
    }
}