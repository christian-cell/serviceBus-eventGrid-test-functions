using netNinja.ServiceBusTests.Models.Commands;

namespace netNinja.ServiceBusTest.Services.Abstractions
{
    public interface IServiceBusTestSendService
    {
        Task<string> SendToServiceBusAsync(ServiceBusMessageCommand serviceBusMessageCommand);
        Task<string> SendToEventToEventGrid(EventGridMessageCommand eventGridMessageCommand);
    }
};

