using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract specific header from the request
            var signedHeader = context.Request.Headers["Api-Gateway"].FirstOrDefault();

            // Null means, the request is not coming from the API Gateway // 503 service unavailable
            if (signedHeader is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Access denied. This endpoint is only accessible through the API Gateway.");
                return;
            }

            await next(context);
        }
    }
}
