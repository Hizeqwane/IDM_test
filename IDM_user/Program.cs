// See https://aka.ms/new-console-template for more information

using IDM;
using IDM_user;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Host.CreateDefaultBuilder()
    .ConfigureLogging((context, logBuilder) => logBuilder.AddSimpleConsole())
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddIDMConnector(context.Configuration);
    }).Build().Run();