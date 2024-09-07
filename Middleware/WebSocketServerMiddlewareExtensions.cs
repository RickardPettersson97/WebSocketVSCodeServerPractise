using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebSocketServer.Middleware
{
    public static class WebSocketServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketServer.middleware.WebSocketServerMiddleware>();
        }

        public static IServiceCollection AddWebSocketServerConnectionManager(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketServerConnectionManager>();
            return services;
        }
    }
}