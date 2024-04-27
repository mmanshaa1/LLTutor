using App.Core.Services.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.PL.Extentions
{
    public static class UserManagerExtension
    {
        public static async Task<T?> FindUserByIdOrUserNameOrEmailOrPhoneAsync<T>(this UserManager<T> accountManager, string IdOrEmailOrPhone) where T : Account
        {
            var res = await accountManager.Users.Where(A => A.Email == IdOrEmailOrPhone || A.PhoneNumber == IdOrEmailOrPhone || A.UserName == IdOrEmailOrPhone || A.Id.ToString() == IdOrEmailOrPhone).FirstOrDefaultAsync();

            if (res is null)
            {
                try
                {
                    res = await accountManager.FindByIdAsync(IdOrEmailOrPhone);
                }
                catch (Exception) { }

                return res;
            }
            return res;
        }

        public static async Task<bool> IsEmailOrPhoneRegisteredAsync<T>(this UserManager<T> accountManager, string EmailOrPhone) where T : Account
            => await accountManager.Users.AnyAsync(A => A.Email == EmailOrPhone || A.PhoneNumber == EmailOrPhone || A.UserName == EmailOrPhone);


        public static async Task<T?> GetUserFromTokenAsync<T>(this UserManager<T> accountManager, ITokenService tokenService, string token) where T : Account
        {
            try
            {
                var email = await tokenService.GetEmailFromTokenAsync(token);
                if (email is null)
                    return null;

                return await accountManager.FindByEmailAsync(email);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
