using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare variables
            string message = "Sorry, internal server error occured. Kindly try again";
            int statusCode = StatusCodes.Status500InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                //check if Exception is too many request // 429 status code
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    message = "Too many requests. Please try again later.";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    title = "Too many requests";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // If response is UnAuthorize // 401 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    message = "Unauthorized access. Please provide valid credentials.";
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Unauthorized";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // If response is Forbidden // 403 status code
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    message = "Forbidden access. You do not have permission to access this resource.";
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Forbidden";
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                // Check if the exception is a TaskCanceledException or TimeoutException // 408 request timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    message = "The request timed out. Please try again later.";
                    statusCode = StatusCodes.Status408RequestTimeout;
                    title = "Request Timeout";
                }

                // If none of the exceptions then do the default || Exception caught in catch block
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
            return;
        }
    }
}
