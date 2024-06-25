using App.Core.Services.JWT;

namespace App.APIs.Controllers
{
    [Authorize]
    public partial class AccountsController : BaseAPIController
    {
        private readonly SignInManager<Account> accountSignInManager;
        private readonly UserManager<Account> accountManager;
        private readonly ITokenService tokenService;
        private readonly IWebHostEnvironment environment;

        public AccountsController(
            SignInManager<Account> accountSignInManager_,
            UserManager<Account> accountManager_,
            IUnitOfWork unitOfWork_,
            ITokenService tokenService_,
            IWebHostEnvironment environment
            ) : base(unitOfWork_)
        {
            accountSignInManager = accountSignInManager_;
            accountManager = accountManager_;
            tokenService = tokenService_;
            this.environment = environment;
        }

        #region Current User
        [Authorize]
        [ProducesResponseType(typeof(AccountDTOwithToken), StatusCodes.Status200OK)]
        [HttpGet("currentuser")]
        public async Task<ActionResult<AccountDTOwithToken>> GetCurrentUser()
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            if (username is null) return NotFound(new ApiResponse(404));

            var user = await accountManager.FindByNameAsync(username);
            if (user is null) return NotFound(new ApiResponse(404));

            var token = await accountManager.GetAuthenticationTokenAsync(user, "TokenProviderName", "TokenName");

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.PhoneNumber2,
                user.Nationality,
                user.dateOfBirth,
                user.Location,
                Gender = user.Gender.Value ? "Male" : "Female",
                IsVerified = user.EmailConfirmed,
                Token = token
            });

        }
        #endregion

        #region Validate Token
        [AllowAnonymous]
        [HttpGet("validateToken")]
        public async Task<ActionResult> ValidateToken(string token)
        {
            try
            {
                var Email = await tokenService.GetEmailFromTokenAsync(token);
                var user = await accountManager.FindByEmailAsync(Email);
                if (user is null)
                    return NotFound(new ApiResponse(404));

                var res = await tokenService.ValidateToken(token);

                if (res)
                {
                    if (user.RefreshToken.IsExpired)
                    {
                        user.RefreshToken = await tokenService.GenerateRefreshTokenAsync();
                        SetRefreshTokenInCookie(user.RefreshToken.Token, user.RefreshToken.ExpiresOn);
                        await accountManager.UpdateAsync(user);
                    }
                    return Ok(new { token });
                }
                var userRefreshToken = user.RefreshToken;
                if (userRefreshToken is null || userRefreshToken.IsExpired || Request.Cookies["refreshToken"] != userRefreshToken.Token)
                    return BadRequest(new ApiResponse(403, "Inactive Token"));

                var newAccessToken = await tokenService.CreateTokenAsync(user, accountManager);
                await accountManager.SetAuthenticationTokenAsync(user, "TokenProviderName", "TokenName", newAccessToken);

                user.RefreshToken = await tokenService.GenerateRefreshTokenAsync();
                SetRefreshTokenInCookie(user.RefreshToken.Token, user.RefreshToken.ExpiresOn);
                await accountManager.UpdateAsync(user);

                return Ok(new { Token = newAccessToken });
            }
            catch (Exception)
            {
                return BadRequest(new ApiResponse(403, "Inactive Token"));
            }
        }

        private void SetRefreshTokenInCookie(string RefreshToken, DateTime expires)
        {
            var CookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken", RefreshToken, CookieOptions);
        }
        #endregion
    }
}
