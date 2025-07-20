

using VituraHealthWebApi.Services.Interfaces;

namespace VituraHealthWebApi.HostedServices;

public class DataLoadingHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataLoadingHostedService> _logger;

    public DataLoadingHostedService(IServiceProvider serviceProvider, ILogger<DataLoadingHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting data loading...");
        
        try
        {
            // Create a scope to resolve services, as you cannot resolve scoped services in singleton services
            using var scope = _serviceProvider.CreateScope();
            var seedingService = scope.ServiceProvider.GetRequiredService<IDataLoadingService>();
            await seedingService.LoadDataAsync();

            _logger.LogInformation("Data loading completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load data during startup");
            throw; // Re-throw to prevent application from starting with invalid state
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Data loading service stopped.");
        return Task.CompletedTask;
    }
}
