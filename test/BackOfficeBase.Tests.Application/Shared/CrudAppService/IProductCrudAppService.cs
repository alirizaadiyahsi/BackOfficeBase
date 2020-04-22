using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.Tests.Application.Shared.CrudAppService.Dto;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;

namespace BackOfficeBase.Tests.Application.Shared.CrudAppService
{
    public interface IProductCrudAppService:ICrudAppService<Product, ProductDto, ProductDto, CreateProductInput, UpdateProductInput>
    {
    }
}