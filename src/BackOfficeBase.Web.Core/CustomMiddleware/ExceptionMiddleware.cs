using System;
using System.Net;
using System.Threading.Tasks;
using BackOfficeBase.Utilities.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace BackOfficeBase.Web.Core.CustomMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                foreach (var exception in ex.GetInnerExceptions())
                {
                    Log.Error(exception.Message);
                    Log.Error(exception.StackTrace);
                }

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(new
            {
                ResultCode = 0,
                ErrorMessage = exception.Message
            });

            return context.Response.WriteAsync(result);
        }
    }
}
