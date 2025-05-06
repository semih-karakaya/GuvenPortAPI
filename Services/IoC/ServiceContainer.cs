using Microsoft.Extensions.DependencyInjection;
using GuvenPortAPI.Service;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Services.IoC
{
    public static class ServiceContainer
    {
        public static void AddScopedService(this IServiceCollection services)
        {
           //add.scoped yapilacak
           services.AddScoped< IOfficeService,OfficeService>();
           services.AddScoped< IWorkplaceService,WorkplaceService>();

        }
    }
}