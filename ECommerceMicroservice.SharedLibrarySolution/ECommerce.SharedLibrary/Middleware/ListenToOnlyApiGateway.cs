using Microsoft.AspNetCore.Http;

namespace ECommerce.SharedLibrary.Middleware;

internal class ListenToOnlyApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var signedHeader = context.Request.Headers["Api-Gateway"];

        // null means, the request is not coming from the API Gateway
        if (signedHeader.FirstOrDefault() is null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Sorry, service is unvaliable");
            return;
        }
        await next(context);

    }
}
