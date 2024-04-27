using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace App.Core.Services.JWT
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        private readonly IConfiguration configuration = configuration;

        public async Task<string> CreateTokenAsync<T>(T MyUser, UserManager<T> userManager) where T : Account
        {
            ArgumentNullException.ThrowIfNull(MyUser);
            ArgumentNullException.ThrowIfNull(userManager);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.GivenName, MyUser.UserName ?? "UserName"),
                new(ClaimTypes.Email, MyUser.Email ?? "Email"),
                new(ClaimTypes.MobilePhone, MyUser.PhoneNumber ?? "PhoneNumber")
            };

            var userRoles = await userManager.GetRolesAsync(MyUser);
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            //Token object
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(double.Parse(configuration["JWT:DurationInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<RefreshToken> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            var refreshToekn = new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.ToLocalTime().AddDays(double.Parse(configuration["JWT:RefreshTokenDurationInDays"]))
            };

            return Task.FromResult(refreshToekn);
        }

        // chck if token is valid
        public async Task<bool> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authKey,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetEmailFromTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                return jwtToken?.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
