using Microsoft.AspNetCore.Http;
using Middlewares.Exceptionfeature;

namespace WebApplication1.Middlewares.ExceptionFeatures
{
    public class HandleExecption
    {
        private RequestDelegate next;

        public HandleExecption(RequestDelegate request)
        {
            this.next = request;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = ex switch
                {
                    ArgumentException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    CustomExecption => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                await httpContext.Response.WriteAsJsonAsync(
                    new { error = ex.Message, status = httpContext.Response.StatusCode });
            }

        }
    }
    }
