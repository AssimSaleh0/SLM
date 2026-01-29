using SLM.Core.Interfaces;
using SLM.Core.Models;
using System.Security.Claims;

namespace SLM.Middleware
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            if (context.Request.Method != "GET")
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var action = $"{context.Request.Method} {context.Request.Path}";
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                var auditLog = new AuditLog
                {
                    UserId = userId != null ? int.Parse(userId) : null,
                    Action = action,
                    EntityName = ExtractEntityName(context.Request.Path),
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                try
                {
                    await unitOfWork.Repository<AuditLog>().AddAsync(auditLog);
                    await unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create audit log");
                }
            }

            await _next(context);
        }

        private static string ExtractEntityName(PathString path)
        {
            var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return segments != null && segments.Length > 2 ? segments[2] : "Unknown";
        }
    }
}