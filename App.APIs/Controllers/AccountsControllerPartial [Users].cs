namespace App.APIs.Controllers
{
    public partial class AccountsController
    {
        #region Account Account Login
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(AccountDTOwithToken), StatusCodes.Status200OK)]
        [HttpPost("login")]
        public async Task<ActionResult<AccountDTOwithToken>> AccountLogin(LoginDto model)
        {
            var account = await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(model.EmailOrPhone);

            if (account is null)
                return Unauthorized(new ApiResponse(401, "Invalid password or email."));

            var res = await accountSignInManager.PasswordSignInAsync(account, model.Password, model.RememberMe != 0, false);

            if (!res.Succeeded)
                return Unauthorized(new ApiResponse(401, "Invalid password or email."));

            #region Tokens Handeling
            if (account.RefreshToken is not null && !account.RefreshToken.IsExpired)
                SetRefreshTokenInCookie(account.RefreshToken.Token, account.RefreshToken.ExpiresOn);

            else
            {
                account.RefreshToken = await tokenService.GenerateRefreshTokenAsync();
                SetRefreshTokenInCookie(account.RefreshToken.Token, account.RefreshToken.ExpiresOn);
                await accountManager.UpdateAsync(account);
            }

            var token = await tokenService.CreateTokenAsync(account, accountManager);
            await accountManager.SetAuthenticationTokenAsync(account, "TokenProviderName", "TokenName", token);
            #endregion

            if (!account.EmailConfirmed)
            {
                await SendOTP(account, EmailOTPType.Verification);
            }

            return Ok(new
            {
                account.FirstName,
                account.LastName,
                account.Email,
                Token = token,
                IsVerified = account.EmailConfirmed
            });
        }

        private static readonly string[] error = new string[] { "Email is already registered for another account!" };
        #endregion

        #region Account Account Logout
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("logout")]
        public async Task<ActionResult> AccountLogout()
        {
            // Get the currently authenticated user
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            if (username is null) return BadRequest(new ApiResponse(400, "Logout failed!"));

            Account? currentUser = (Account?)await accountManager.FindByNameAsync(username);
            if (currentUser is null) return BadRequest(new ApiResponse(400, "Logout failed!"));
            var token = await accountManager.GetAuthenticationTokenAsync(currentUser, "TokenProviderName", "TokenName");

            currentUser.RefreshToken = null;
            if (token != null)
            {
                // Clear the authentication token
                await accountSignInManager.SignOutAsync();
                await accountManager.RemoveAuthenticationTokenAsync(currentUser, "TokenProviderName", "TokenName");
                return Ok(new ApiResponse(200, "Logout done successfully!"));
            }

            return Ok(new ApiResponse(200, "You have already logged out!"));

        }
        #endregion

        #region Account Account Change Password
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [HttpPut("changepassword")]
        public async Task<ActionResult> AccountChangePassword(ChangePasswordDTO model)
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            if (username is null)
                return NotFound(new ApiResponse(404));
            var user = await accountManager.FindByNameAsync(username);
            if (user is null)
                return NotFound(new ApiResponse(404));

            var result = await accountManager.ChangePasswordAsync(user, model.current_password, model.new_password);
            if (result.Succeeded)
                return Ok(new ApiResponse(200, "Password changed successfully."));

            return BadRequest(new ApiResponse(400, "Failed to change the password, please ensure that the current password is correct!"));

        }
        #endregion

        #region Account Account Update Profile
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [HttpPut("updatecurrent")]
        public async Task<ActionResult> UpdateCurrentAccount(UpdateAccountDTO updatedAccount)
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            if (username == null)
                return BadRequest(new ApiResponse(400, "User update failed, please make sure the current password is correct!"));

            var account = await accountManager.FindByNameAsync(username);

            if (account == null)
                return BadRequest(new ApiResponse(400, "User update failed, please make sure the current password is correct!"));

            if (!string.IsNullOrEmpty(updatedAccount.FirstName))
                account.FirstName = updatedAccount.FirstName;

            if (!string.IsNullOrEmpty(updatedAccount.Nationality))
                account.Nationality = updatedAccount.Nationality;

            if (!string.IsNullOrEmpty(updatedAccount.LastName))
                account.LastName = updatedAccount.LastName;

            if (!string.IsNullOrEmpty(updatedAccount.PhoneNumber))
                account.PhoneNumber = updatedAccount.PhoneNumber;

            if (!string.IsNullOrEmpty(updatedAccount.PhoneNumber2))
                account.PhoneNumber2 = updatedAccount.PhoneNumber2;

            if (updatedAccount.dateOfBirth != null)
                account.dateOfBirth = updatedAccount.dateOfBirth;

            if (!string.IsNullOrEmpty(updatedAccount.Location))
                account.Location = updatedAccount.Location;

            if (!string.IsNullOrEmpty(updatedAccount.Gender))
                account.Gender = updatedAccount.Gender == "M";

            if (!string.IsNullOrEmpty(updatedAccount.Email) && updatedAccount.Email != account.Email)
            {
                if (!Regex.IsMatch(updatedAccount.Email, @"^[a-zA-Z0-9_.+-]+@(gmail|yahoo|hotmail|outlook|live|icloud)\.(com|net|org)$"))
                    return BadRequest(new ApiValidationErrorResponse() { Errors = ["Email must be of type gmail, yahoo, hotmail, outlook, live, icloud"] });

                if (await accountManager.IsEmailOrPhoneRegisteredAsync(updatedAccount.Email))
                    return BadRequest(new ApiValidationErrorResponse() { Errors = error });

                account.Email = updatedAccount.Email;
                account.EmailConfirmed = false;
                await SendOTP(account, EmailOTPType.Verification);
            }
            var result = await accountManager.UpdateAsync(account);

            if (result.Succeeded)
                return Ok(new ApiResponse(200, "Your information has been updated successfully"));

            return BadRequest(new ApiResponse(404, "User not found"));

        }
        #endregion

        #region Register Account
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AccountDTO), StatusCodes.Status200OK)]
        [HttpPost("register")]
        public async Task<ActionResult<AccountDTO>> RegisterAccount(RegisterAccountDTO model)
        {
            #region Check Email and Phone Number
            if (await accountManager.IsEmailOrPhoneRegisteredAsync(model.Email))
                return BadRequest(new ApiValidationErrorResponse() { Errors = ["يبدو أن لديك بالفعل حسابًا باستخدام هذا البريد الإلكتروني، يرجى تسجيل الدخول!"] });

            if (await accountManager.IsEmailOrPhoneRegisteredAsync(model.PhoneNumber))
                return BadRequest(new ApiValidationErrorResponse() { Errors = ["يبدو أن لديك بالفعل حسابًا باستخدام هذا البريد الإلكتروني، يرجى تسجيل الدخول!"] });

            if (!Regex.IsMatch(model.Email, @"^[a-zA-Z0-9_.+-]+@(gmail|yahoo|hotmail|outlook|live|icloud)\.(com|net|org)$"))
                return BadRequest(new ApiValidationErrorResponse() { Errors = ["يجب أن يكون البريد الإلكتروني من نوع gmail, yahoo, hotmail, outlook, live, icloud"] });

            if (model.PhoneNumber.Length != 11)
                return BadRequest(new ApiValidationErrorResponse() { Errors = ["يجب أن يكون رقم الهاتف 11 رقمًا!"] });

            #endregion

            var account = new Account()
            {
                FirstName = model.FirstName,
                Nationality = model.Nationality,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                Gender = model.Gender == "M"
            };

            var res = await accountManager.CreateAsync(account, model.Password);

            if (!res.Succeeded)
                return BadRequest(new ApiValidationErrorResponse() { Errors = ["Error in password validation"] });

            await SendOTP(account, EmailOTPType.Verification);

            return Ok(new ApiResponse(200, "Account has been successfully registered."));

        }
        #endregion

    }
}
