using SLM.Middleware;

namespace SLM.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuditLoggingMiddleware>();
        }
    }
}