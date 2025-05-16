using Microsoft.Extensions.DependencyInjection;
using GuvenPortAPI.Service;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Services.IoC
{
    public static class ServiceContainer
    {
        public static void AddScopedService(this IServiceCollection services)
        {
            // Add all services here
            services.AddScoped<IOfficeService, OfficeService>();
            services.AddScoped<IWorkplaceService, WorkplaceService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IAccidentService, AccidentService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IMedicalExaminationService, MedicalExaminationService>();
            // Add other services as needed
        }
    }
}