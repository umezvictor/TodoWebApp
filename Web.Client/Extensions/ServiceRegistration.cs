using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Client.ActionFilters;
using Web.Client.Data;
using Web.Client.Models;
using Web.Client.Services;

namespace Web.Client.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddSetup(this IServiceCollection services, IConfiguration configuration)
        {
            
                services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                   configuration.GetConnectionString("Default"),
                   b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));


            services.AddTransient<IUserManagementService, UserManagementService>();
            services.AddTransient<IAuthService, AuthService>();        
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ITodoService, TodoService>();
            services.AddScoped<AuthenticationFilter>();

            services.Configure<IdentityOptions>(options =>
            {
                //Password Settings
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                // Lockout settings.
               // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);                
                //options.Lockout.AllowedForNewUsers = true;

            });

            services.AddIdentity<AppUser, AppRole>()
          .AddEntityFrameworkStores<AppDbContext>()
          .AddDefaultTokenProviders();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);

            });
        }
    }
}
