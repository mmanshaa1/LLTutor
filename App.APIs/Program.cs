using App.Repository.Data.Contexts;

namespace App.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services

            // configure Json Serializer to ignore Reference Loop Handling
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // for DbContext
            builder.Services.AddDbContext<MainContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
                //options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            builder.Services.IdentityServices(builder.Configuration);

            builder.Services.AddSwaggerServices();

            builder.Services.AddApplicationServices(builder.Configuration);

            #endregion

            var app = builder.Build();

            #region Seeding Data and Apply Migrations if exist, Then Update Database & Log Exception if exist also.

            // using: to dispose connection
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                // ASK CLR To Create dbContext Explicitly
                var dbContext = services.GetRequiredService<MainContext>();

                // If any Migrations created, Apply it, then Update-Database
                // await dbContext.Database.MigrateAsync();

                // Seeding Data 
                await MainContextSeed.SeedAsync(dbContext);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error Occured During apply the migration");
            }
            #endregion

            #region Configure Kestrel Middlewares
            // To Handle Exception Error [Internal Server Error]
            app.UseMiddleware<ExceptionMiddleware>();

            // Enable Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
            }

            // To Handle Not-Found End-Point Error
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseStaticFiles();

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/images/ProfilePics")),
            //    RequestPath = "/images"
            //});

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}