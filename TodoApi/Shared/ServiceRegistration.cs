
namespace Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services)
        {
          
            services.AddTransient<IDateTimeService, DateTimeService>();
            
        }
    }
}
