using App.Core.Services.JWT;
using App.Repository.Data.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace App.PL.Extentions
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
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                //options.Lockout.MaxFailedAccessAttempts = 6;
                //options.Lockout.AllowedForNewUsers = true;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<MainContext>()
            .AddSignInManager<SignInManager<Account>>();

            Services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromDays(1) // token expiration time                
            );

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

            Services.AddMvc();

            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
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
            .AddCookie("JWTScheme", options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/Logout";
            });

            Services.AddAuthorization(options =>
            {
                options.AddPolicy("JWTPolicy", policy =>
                {
                    policy.AddRequirements(new JWTAuthRequirement());
                    policy.AddAuthenticationSchemes("JWTScheme");
                });

                options.DefaultPolicy = options.GetPolicy("JWTPolicy");
                options.FallbackPolicy = options.GetPolicy("JWTPolicy");
            });

            Services.AddScoped<IAuthorizationHandler, JWTAuthHandler>();

            return Services;
        }
    }
}
