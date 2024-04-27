using App.Core.Services.JWT;
using Microsoft.AspNetCore.Authorization;

namespace App.PL.Extentions
{
    public class JWTAuthRequirement : IAuthorizationRequirement
    {
        public string CookieName { get; }
        public JWTAuthRequirement(string sessionHeaderName = "JWToken")
        {
            CookieName = sessionHeaderName;
        }
    }

    public class JWTAuthHandler : AuthorizationHandler<JWTAuthRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;

        public JWTAuthHandler(IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JWTAuthRequirement requirement)
        {
            var httpRequest = _httpContextAccessor.HttpContext!.Request;

            if (string.IsNullOrEmpty(httpRequest.Cookies[requirement.CookieName]))
                context.Fail();
            else
            {
                var cookieValue = httpRequest.Cookies[requirement.CookieName];
                var isValid = await _tokenService.ValidateToken(cookieValue);

                if (isValid)
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
        }
    }
}
