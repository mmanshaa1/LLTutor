namespace App.APIs.Controllers
{
    public partial class AccountsController
    {
        #region OPT to Reset Password & Verify Account

        #region Send OTP
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [HttpPost("SendOTPResetPassword")]
        public async Task<ActionResult> SendOTPResetPassword(string email)
        {
            Account? user = (Account?)await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(email);
            if (user is null)
                return NotFound(new ApiResponse(404, "Invalid data, user not found!"));

            return await SendOTP(user, EmailOTPType.ResetPassword);
        }

        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [HttpPost("SendOTPConfirmAccount")]
        public async Task<ActionResult> SendOTPConfirmAccount()
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            Account? User_ = (Account?)await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(username);
            if (User_ is null)
                return NotFound(new ApiResponse(404, "Invalid data, user not found!"));

            if (string.IsNullOrEmpty(User_.Email))
                return BadRequest(new ApiResponse(400, "The user has no email or invalid data!"));

            return await SendOTP(User_, EmailOTPType.Verification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> SendOTP(Account account, EmailOTPType emailType)
        {
            var spec = new OTPSpec(O => O.AccountId == account.Id);
            var StoredOTP = await unitOfWork.Repository<OTPForConfirm>().GetByIdWithSpecAsync(spec);

            if (StoredOTP != null)
            {
                var time = DateTime.Now.TimeOfDay.Subtract(StoredOTP.LifeTime);
                if (time.TotalMinutes > 5)
                {
                    var OTP = RandomGenerator.GenerateNumericOTP(6);
                    var Toemail = new EmailDTOForOTP()
                    {
                        Type = emailType,
                        ToEmail = account.Email,
                        OTP = OTP
                    };

                    await EmailService.SendEmailHtmlOTP(Toemail);

                    StoredOTP.OTPHashed = Hash.GenerateHashCode(OTP.ToString());
                    StoredOTP.LifeTime = DateTime.Now.TimeOfDay;
                    unitOfWork.Repository<OTPForConfirm>().Update(StoredOTP);
                    await unitOfWork.CompleteAsync();

                    return Ok(new ApiResponse(200, "Verification code sent successfully"));

                }
                else
                    return BadRequest(new ApiResponse(400, "Check your inbox"));

            }
            else
            {
                var OTP = RandomGenerator.GenerateNumericOTP(6);
                var Toemail = new EmailDTOForOTP()
                {
                    Type = emailType,
                    ToEmail = account.Email,
                    OTP = OTP.ToString()
                };
                await EmailService.SendEmailHtmlOTP(Toemail);
                await unitOfWork.Repository<OTPForConfirm>().AddAsync(new OTPForConfirm()
                {
                    AccountId = account.Id,
                    OTPHashed = Hash.GenerateHashCode(OTP.ToString()),
                    LifeTime = DateTime.Now.TimeOfDay
                });
                await unitOfWork.CompleteAsync();

                return Ok(new ApiResponse(200, "Verification code sent successfully"));
            }
        }

        #endregion

        #region Confirm OTP
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [HttpPost("ConfirmAccountOTP")]
        public async Task<ActionResult> ConfirmOTPAccount(string OTP)
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            Account? user = (Account?)await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(username);
            if (user is not null)
            {
                OTPSpec? spec = new OTPSpec(O => O.AccountId == user.Id);
                var StoredOTP = await unitOfWork.Repository<OTPForConfirm>().GetByIdWithSpecAsync(spec);

                if (StoredOTP is not null)
                {
                    var time = DateTime.Now.TimeOfDay.Subtract(StoredOTP.LifeTime);
                    if (time.TotalMinutes > 10)
                    {
                        unitOfWork.Repository<OTPForConfirm>().Delete(StoredOTP);
                        await unitOfWork.CompleteAsync();
                        return BadRequest(new ApiResponse(400, "Verification code has expired"));

                    }
                    else if (StoredOTP.OTPHashed == Hash.GenerateHashCode(OTP.ToString()))
                    {
                        var Validationtoken = await accountManager.GenerateEmailConfirmationTokenAsync(user);
                        IdentityResult? result = await accountManager.ConfirmEmailAsync(user, Validationtoken);

                        if (result.Succeeded)
                        {
                            unitOfWork.Repository<OTPForConfirm>().Delete(StoredOTP);
                            await unitOfWork.CompleteAsync();
                            return Ok(new ApiResponse(200, "Account has been successfully confirmed"));

                        }
                    }
                    return BadRequest(new ApiValidationErrorResponse() { message = "Invalid verification code" });

                }
                return BadRequest(new ApiResponse(400, "The user did not request a verification code"));

            }

            return BadRequest(new ApiValidationErrorResponse() { message = "User not found" });

        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
        [HttpPost("ConfirmResetPasswordOTP")]
        public async Task<ActionResult> ConfirmOTPPassword(string email, int OTP)
        {
            Account? User = (Account?)await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(email);
            if (User is not null)
            {
                var spec = new OTPSpec(O => O.AccountId == User.Id);
                var StoredOTP = await unitOfWork.Repository<OTPForConfirm>().GetByIdWithSpecAsync(spec);
                if (StoredOTP is not null)
                {
                    var time = DateTime.Now.TimeOfDay.Subtract(StoredOTP.LifeTime);
                    if (time.TotalMinutes > 30)
                    {
                        unitOfWork.Repository<OTPForConfirm>().Delete(StoredOTP);
                        await unitOfWork.CompleteAsync();
                        return BadRequest(new ApiResponse(400, "Verification code has expired"));
                    }
                    else if (StoredOTP.OTPHashed == Hash.GenerateHashCode(OTP.ToString()))
                    {
                        string token = await accountManager.GeneratePasswordResetTokenAsync(User);

                        unitOfWork.Repository<OTPForConfirm>().Delete(StoredOTP);
                        await unitOfWork.CompleteAsync();
                        return Ok(new
                        {
                            code = 200,
                            message = "Use this code to reset your password.",
                            token
                        });
                    }

                    return BadRequest(new ApiResponse(400, "Invalid verification code"));

                }
                return BadRequest(new ApiResponse(400, "The user did not request a verification code or it has expired"));

            }
            return BadRequest(new ApiResponse(400, "User not found!"));

        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDTO resetPassword)
        {
            Account? User = (Account?)await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(resetPassword.Email);
            if (User is not null)
            {
                IdentityResult? result = await accountManager.ResetPasswordAsync(User, resetPassword.token, resetPassword.new_password);

                if (result.Succeeded)
                    return Ok(new ApiResponse(200, "Password reset successfully!"));

                else
                    return BadRequest(new ApiResponse(400, "Invalid data, user not found"));

            }
            return BadRequest(new ApiResponse(400, "User not found!"));

        }
        #endregion

        #endregion


    }
}
