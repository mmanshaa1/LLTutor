
using App.Core.Interfaces;
using App.Repository;

namespace App.PL.Extentions
{
    public static class AddApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            GlobalVariables.Initialize(configuration);
            EmailSettings.Initialize(configuration);

            return Services;
        }
    }
}
