using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
namespace App.Core.Services.JWT
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync<T>(T MyUser, UserManager<T> userManager) where T : Account;
        Task<bool> ValidateToken(string token);
        public Task<RefreshToken> GenerateRefreshTokenAsync();
        public Task<string> GetEmailFromTokenAsync(string token);
    }
}
