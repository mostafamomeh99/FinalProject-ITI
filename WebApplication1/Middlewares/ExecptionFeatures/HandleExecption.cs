using System.Net.Http;
using WebApplication1.Middlewares.ExceptionFeatures;

namespace WebApplication1.Middlewares.ExecptionFeatures
{
    public class HandleExecption
    {
        private RequestDelegate next;

        public HandleExecption(RequestDelegate request)
        {
            next = request;
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
                    new { message = ex.Message, status = httpContext.Response.StatusCode });
            }

        }
    }
    }
