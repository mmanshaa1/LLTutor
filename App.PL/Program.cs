using App.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace App.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<MainContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
                //options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            #region Drop all tables in the database
            //using (var dbContext = new MainContext(builder.Services.BuildServiceProvider().GetService<DbContextOptions<MainContext>>()))
            //{
            //    await dbContext.DropAllTablesAsync();
            //}
            #endregion

            builder.Services.IdentityServices(builder.Configuration);

            builder.Services.AddApplicationServices(builder.Configuration);

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                // ASK CLR To Create dbContext Explicitly
                var dbContext = services.GetRequiredService<MainContext>();

                // If any Migrations created, Apply it, then Update-Database
                await dbContext.Database.MigrateAsync();

                // Seeding Data
                await MainContextSeed.SeedAsync(dbContext);

                //Roles
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // seeding Roles
                await MainContextSeed.SeedingRoles(roleManager);

                // ASK CLR To Create userManager Explicitly
                var accountManager = services.GetRequiredService<UserManager<Account>>();

                // seeding Master Admin
                await MainContextSeed.SeedingMasterAdmin(accountManager);
            }
            catch (Exception ex)
            {
                // Logging Error to Console [Kestrel]
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error Occured During apply the migration");
            }

            app.UseStatusCodePagesWithRedirects("/Error");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}