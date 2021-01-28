using Microsoft.AspNetCore.Builder;

namespace ToDoList.API.Middlewares.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
