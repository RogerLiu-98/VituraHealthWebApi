using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VituraHealthWebApi.DataAccess.Contexts;
using VituraHealthWebApi.DataAccess.Repositories;
using VituraHealthWebApi.DataAccess.Repositories.Interfaces;
using VituraHealthWebApi.HostedServices;
using VituraHealthWebApi.Services;
using VituraHealthWebApi.Services.Interfaces;

namespace VituraHealthWebApi;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Entity Framework with In-Memory database
        services.AddDbContext<VituraHealthDbContext>(options =>
            options.UseInMemoryDatabase("VituraHealthDb"));

        // Register services
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IPrescriptionService, PrescriptionService>();

        // Repositories
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();

        // Data Loading Service
        services.AddScoped<IDataLoadingService, DataLoadingService>();

        // Register the data loading service
        services.AddHostedService<DataLoadingHostedService>();

        return services;
    }
}
