using App.Core.Services.JWT;

namespace App.PL.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<Account> _signInManager;
        private readonly UserManager<Account> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(SignInManager<Account> signInManager, UserManager<Account> userManager, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        #region Login
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            var accessToken = Request.Cookies["JWToken"];
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                var isValid = await _tokenService.ValidateToken(accessToken);
                if (isValid)
                    //return RedirectToAction("Index", "Courses");
                    return View("LooksGood");
                else if (!string.IsNullOrEmpty(refreshToken))
                {
                    var admin = await _userManager.GetUserFromTokenAsync(_tokenService, accessToken);

                    if (admin is not null && admin.RefreshToken is not null && !admin.RefreshToken.IsExpired && admin.RefreshToken.Token == refreshToken)
                    {
                        var token = await _tokenService.CreateTokenAsync(admin, _userManager);
                        RemoveFromCookies("JWToken");
                        SetInCookies("JWToken", token, admin.RefreshToken.ExpiresOn, true);
                        await _userManager.SetAuthenticationTokenAsync(admin, "TokenProviderName", "TokenName", token);
                        //return RedirectToAction("Index", "Courses");
                        return View("LooksGood");
                    }
                }
            }

            RemoveFromCookies("JWToken");
            RemoveFromCookies("refreshToken");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = await _userManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(model.Email);
                if (admin is null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View(model);
                }

                var res = await _signInManager.PasswordSignInAsync(admin, model.Password, model.RememberMe, true);
                if (!res.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View(model);
                }

                if (admin.RefreshToken is not null && !admin.RefreshToken.IsExpired)
                    SetInCookies("refreshToken", admin.RefreshToken.Token, admin.RefreshToken.ExpiresOn, model.RememberMe);
                else
                {
                    admin.RefreshToken = await _tokenService.GenerateRefreshTokenAsync();
                    SetInCookies("refreshToken", admin.RefreshToken.Token, admin.RefreshToken.ExpiresOn, model.RememberMe);
                    await _userManager.UpdateAsync(admin);
                }

                var token = await _tokenService.CreateTokenAsync(admin, _userManager);
                SetInCookies("JWToken", token, admin.RefreshToken.ExpiresOn, model.RememberMe);
                await _userManager.SetAuthenticationTokenAsync(admin, "TokenProviderName", "TokenName", token);
                //return RedirectToAction("Index", "Home");
                return View("LooksGood");
            }
            return View(model);
        }
        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            RemoveFromCookies("JWToken");
            RemoveFromCookies("refreshToken");
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Profile
        public async Task<IActionResult> Profile()
        {
            var accessToken = Request.Cookies["JWToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                var email = await _tokenService.GetEmailFromTokenAsync(accessToken);
                var admin = await _userManager.FindByEmailAsync(email);

                if (admin is not null)
                {
                    return View(new LoggedInUser()
                    {
                        Id = admin.Id,
                        Email = admin.Email,
                        Name = admin.Name,
                        Phone = admin.PhoneNumber,
                        UserName = admin.UserName,
                        EmailConfirmed = admin.EmailConfirmed
                    });
                }
            }
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(LoggedInUser currentUser)
        {
            if (ModelState.IsValid)
            {
                var accessToken = Request.Cookies["JWToken"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var email = await _tokenService.GetEmailFromTokenAsync(accessToken);
                    var admin = await _userManager.FindByEmailAsync(email);

                    if (admin is not null)
                    {
                        admin.Name = currentUser.Name;
                        admin.Email = currentUser.Email;
                        admin.PhoneNumber = currentUser.Phone;
                        admin.UserName = currentUser.UserName;

                        await _userManager.UpdateAsync(admin);
                        TempData["Message"] = "Account Updated Successfully!";
                    }
                    return View("Profile");
                }
            }
            return RedirectToAction(nameof(Profile), currentUser);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromForm] string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest();
            }

            try
            {
                var accessToken = Request.Cookies["JWToken"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var admin = await _userManager.GetUserFromTokenAsync(_tokenService, accessToken);

                    if (admin is not null)
                    {
                        await _userManager.RemovePasswordAsync(admin);
                        await _userManager.AddPasswordAsync(admin, password);
                        await _userManager.UpdateAsync(admin);

                        TempData["Message"] = "Password Updated Successfully!";
                    }
                    return Ok("Password Updated Successfully!");
                }
                return RedirectToAction(nameof(Login));
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Manage Cookies
        private void SetInCookies(string Key, string Value, DateTime expires, bool RememberMe)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                Expires = RememberMe ? expires : null
            };

            Response.Cookies.Append(Key, Value, cookieOptions);
        }

        private void RemoveFromCookies(string Key)
        {
            Response.Cookies.Delete(Key);
        }

        #endregion
    }
}
