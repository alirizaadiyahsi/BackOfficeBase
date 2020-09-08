using BackOfficeBase.Web.Core.CustomMiddleware;
using Microsoft.AspNetCore.Builder;

namespace BackOfficeBase.Web.Core.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
