using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NetNinja.EventGridDealer.Configuration;
using NetNinja.EventGridDealer.Contracts;
using NetNinja.EventGridDealer.Implementations;
using NetNinja.ServiceBusDealer.Configurations;
using netNinja.ServiceBusTest.Services;
using netNinja.ServiceBusTests.Models.Commands;
using NetNinja.ServiceBusDealer.Contracts;
using NetNinja.ServiceBusDealer.Implementations;

/*
 * debug function https://egwebhook.scm.azurewebsites.net/DebugConsole
 * eliminar variable de la function WEBSITE_RUN_FROM_PACKAGE para que no sea solo readonly
 */

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var serviceBusConfiguration = new ServiceBusConfiguration
        {
            ConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING") ?? string.Empty,
            QueueName = Environment.GetEnvironmentVariable("SERVICEBUS_QUEUENAME") ?? string.Empty
        };

        var eventGridConfiguration = new EventGridConfiguration
        {
            TopicEndpoint = new Uri(Environment.GetEnvironmentVariable("EVENTGRID_TOPICENDPOINT")!) ,
            TopicKey = Environment.GetEnvironmentVariable("EVENTGRID_TOPICKEY") ?? string.Empty
        };

        services.AddSingleton(eventGridConfiguration);

        services.AddSingleton(serviceBusConfiguration);
        services.AddTransient<IServiceBusClientWrapper<ServiceBusMessageCommand>, ServiceBusClientWrapper<ServiceBusMessageCommand>>();
        services.AddTransient<IEventGridClientWrapper<EventGridMessageCommand>, EventGridClientWrapper<EventGridMessageCommand>>();
        
        services.AddApplicationServices();
        
        services.Configure<OpenApiInfo>(options =>
        {
            options.Version = "v1";
            options.Title = "Azure Function API con Swagger";
            options.Description = "Testing of send message to azure serviceBus by netNinja.ServiceBus client";
            options.TermsOfService = new Uri("https://example.com/terms");
            options.Contact = new OpenApiContact
            {
                Name = "Christian",
                Email = "cristohp74@outlook.com",
                Url = new Uri("https://example.com/contact"),
            };
            options.License = new OpenApiLicense
            {
                Name = "Licencia de Uso",
                Url = new Uri("https://example.com/license"),
            };
        });
    })
    .Build();

host.Run();