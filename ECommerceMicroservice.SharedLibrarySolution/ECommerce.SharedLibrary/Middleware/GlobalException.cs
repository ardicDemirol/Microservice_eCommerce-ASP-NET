using ECommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.SharedLibrary.Middleware;
public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Default values
        string messsage = "Sorry,internal server error occured. Kindly try again";
        //int statusCode = (int)HttpStatusCode.InternalServerError;
        int statusCode = StatusCodes.Status500InternalServerError;
        string title = "Error";

        try
        {
            await next(context);

            // check if response is too many request // 429 status code
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                messsage = "Too many request, kindly try again later";
                statusCode = StatusCodes.Status429TooManyRequests;
                await ModifyHeader(context, title, messsage, statusCode);
            }


            // if response is unauthorized // 401 status code
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                messsage = "Sorry, you are not authorized to access this resource";
                statusCode = StatusCodes.Status401Unauthorized;
                await ModifyHeader(context, title, messsage, statusCode);
            }


            // if response is forbidden // 403 status code
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of Access";
                messsage = "Sorry, you are forbidden to access this resource";
                statusCode = StatusCodes.Status403Forbidden;
                await ModifyHeader(context, title, messsage, statusCode);
            }

        }
        catch (Exception ex)
        {
            // log original exceptions /File,debugger,console
            LogException.LogExceptions(ex);

            // check if expression is Timeout // 408 request timeout
            if (ex is TaskCanceledException || ex is TimeoutException)
            {
                title = "Timeout";
                messsage = "Sorry, request timeout. Kindly try again";
                statusCode = StatusCodes.Status408RequestTimeout;
            }

            // if exceptions is caught.
            // if none of the exceptions then do the default
            await ModifyHeader(context, title, messsage, statusCode);
        }
    }

    private static async Task ModifyHeader(HttpContext context, string title, string messsage, int statusCode)
    {
        // display scary-free message to client
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
        {
            Detail = messsage,
            Status = statusCode,
            Title = title
        }), CancellationToken.None);

        return;
    }
}
