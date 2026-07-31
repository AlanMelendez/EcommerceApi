using Ecommerce.Application.Common.Interfaces;
using System.Security.Claims;

namespace Ecommerce.Api.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        //This gives the service access to the current HTTP request.
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //After JWT authentication succeeds, this property contains the authenticated user.
        private ClaimsPrincipal? CurrentUser => _httpContextAccessor.HttpContext?.User;

        public string? UserId => CurrentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? Email => CurrentUser?.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated => CurrentUser?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return CurrentUser?.IsInRole(role) ?? false;
        }
    }
}
