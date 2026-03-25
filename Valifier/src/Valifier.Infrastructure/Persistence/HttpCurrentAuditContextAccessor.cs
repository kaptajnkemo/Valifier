using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Valifier.Infrastructure.Persistence;

public sealed class HttpCurrentAuditContextAccessor : ICurrentAuditContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentAuditContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetActorIdentifier()
    {
        var actorIdentifier = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return string.IsNullOrWhiteSpace(actorIdentifier) ? "Anonymous" : actorIdentifier;
    }

    public string GetTenantIdentifier()
    {
        var tenantIdentifier = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;

        return string.IsNullOrWhiteSpace(tenantIdentifier) ? "Platform" : tenantIdentifier;
    }
}
