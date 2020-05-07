using System.Collections.Generic;
using System.Threading.Tasks;
using BackOfficeBase.Tests.Shared.DataAccess.Entities;
using BackOfficeBase.Web.Core.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace BackOfficeBase.Tests.Web.Core.ActionFilters
{
    public class UnitOfWorkFilterTest : WebCoreTestBase
    {
        [Fact]
        public async Task Should_UnitOfWork_Action_Filter_Save_Changes()
        {
            var product = new Product
            {
                Name = "A Product Name",
                Code = "a_product_name"
            };
            
            var addedProduct = await DefaultTestDbContext.Products.AddAsync(product);
            var dbContextFromAnotherScope = GetDefaultTestDbContext();
            var insertedTestRole = await dbContextFromAnotherScope.Products.FindAsync(addedProduct.Entity.Id);
            Assert.Null(insertedTestRole);

            var actionContext = new ActionContext(
                new DefaultHttpContext
                {
                    Request =
                    {
                        Method = "Post"
                    }
                },
                new RouteData(),
                new ActionDescriptor()
            );
            var actionExecutedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), null);
            var unitOfWorkActionFilter = new UnitOfWorkActionFilter(DefaultTestDbContext);
            
            unitOfWorkActionFilter.OnActionExecuted(actionExecutedContext);
            insertedTestRole = await dbContextFromAnotherScope.Products.FindAsync(addedProduct.Entity.Id);
            Assert.NotNull(insertedTestRole);
        }
    }
}
