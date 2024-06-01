using IDM_Connector;
using IDM.Services;
using IDM.Services.Interfaces;
using IDM.Services.TestServices;
using IDM.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IDM;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIDMConnector(this IServiceCollection services, IConfiguration configuration)
    {
        var testSection = configuration.GetSection(nameof(TestSettings));
        if (testSection.Get<TestSettings>()?.On == true)
            services
                .AddSingleton<IDataProvider, TestDataProvider>()
                .AddOptions<TestSettings>().Bind(testSection);
        else
            services
                .AddSingleton<IDataProvider, DataProvider>()
                .AddHttpClient();

        services
            .AddScoped<IConnector, Connector>()
            .AddSingleton<IValidator, Validator>()
            .AddSingleton<IArchiver, Archiver>();

        var connectorSettingsSection = configuration.GetSection(nameof(ConnectorSettings));
        services.AddOptions<ConnectorSettings>().Bind(connectorSettingsSection);
        
        return services;
    }
}