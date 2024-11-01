using Microsoft.Extensions.DependencyInjection;
using netNinja.ServiceBusTest.Services.Abstractions;
using netNinja.ServiceBusTest.Services.Services;
using netNinja.ServiceBusTests.Models.Commands;

namespace netNinja.ServiceBusTest.Services
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IServiceBusTestSendService, ServiceBusTestSendService>();
            
            return services;
        }
        
    }
};

