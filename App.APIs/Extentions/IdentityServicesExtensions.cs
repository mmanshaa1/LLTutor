using App.Core.Services.JWT;
using App.Repository.Data.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace App.APIs.Extentions
{
    public static class IdentityServicesExtensions
    {
        public static IServiceCollection IdentityServices(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped<ITokenService, TokenService>();

            Services.AddIdentity<Account, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<MainContext>()
            .AddSignInManager<SignInManager<Account>>();

            Services.AddCors();

            Services.AddCors(options =>
            {
                options.AddDefaultPolicy(options =>
                {
                    options.AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowAnyOrigin();
                });
            });

            Services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromHours(3)
            );

            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/errors/401";
                    options.AccessDeniedPath = "/errors/{0}";

                })
                .AddCookie("Forbidden, You don’t have permission to access!", options =>
                {
                    options.AccessDeniedPath = "/errors/401";
                });

            return Services;
        }
    }
}
