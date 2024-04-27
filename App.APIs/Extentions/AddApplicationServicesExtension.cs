using App.Repository;

namespace App.APIs.Extentions
{
    public static class AddApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            EmailService.Initialize(configuration);

            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count > 0)
                                .SelectMany(P => P.Value.Errors).Select(E => E.ErrorMessage).ToList();

                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                });
            });

            return Services;
        }
    }
}
