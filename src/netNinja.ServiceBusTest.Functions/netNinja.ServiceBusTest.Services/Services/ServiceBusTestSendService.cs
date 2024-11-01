using Microsoft.Extensions.Logging;
using NetNinja.EventGridDealer.Contracts;
using netNinja.ServiceBusTest.Services.Abstractions;
using netNinja.ServiceBusTests.Models.Commands;
using NetNinja.ServiceBusDealer.Contracts;

namespace netNinja.ServiceBusTest.Services.Services
{
    public class ServiceBusTestSendService : IServiceBusTestSendService
    {
        private readonly ILogger<ServiceBusTestSendService> _logger;
        private readonly IServiceBusClientWrapper<ServiceBusMessageCommand> _serviceBusClientWrapper;
        private readonly IEventGridClientWrapper<EventGridMessageCommand> _eventGridClientWrapper;
        public ServiceBusTestSendService(
            ILogger<ServiceBusTestSendService> logger,
            IServiceBusClientWrapper<ServiceBusMessageCommand> serviceBusClientWrapper,
            IEventGridClientWrapper<EventGridMessageCommand> eventGridClientWrapper
            )
        {
            _logger = logger;
            _serviceBusClientWrapper = serviceBusClientWrapper;
            _eventGridClientWrapper = eventGridClientWrapper;
        }

        public async Task<string> SendToServiceBusAsync(ServiceBusMessageCommand serviceBusMessageCommand)
        {
            try
            {
                var message = new ServiceBusMessageCommand { Message = "great day let's code", Emissor = "Robert", Receptor = "Claris" };
                // var messages = BuildMessagesForServiceBus();
                
                await _serviceBusClientWrapper.SendMessageAsync(message);
                /*await _serviceBusClientWrapper.SendMessagesAsync(messages);
                await _serviceBusClientWrapper.SendListAsMessage(messages);
                await _serviceBusClientWrapper.SendBatchOfMessagesAsync(messages);
                await _serviceBusClientWrapper.HandleMessage(0, "test", "test");*/
                
                return "Message sent successfully";
            }
            catch (Exception e)
            {
                _logger.LogError($"💀 Exception thrown by nuget is : {e.Message}");
                throw;
            }
        }
        
        public async Task<string> SendToEventToEventGrid(EventGridMessageCommand eventGridMessageCommand)
        {
            try
            {
                var message = new EventGridMessageCommand { Message = "great day let's code", Emissor = "Robert", Receptor = "Claris" };
                var messages = BuildMessagesForEventGrid();
                
                // _eventGridClientWrapper.AuthBySasCredential(4);
                //eventype "netninjatest"

                await _eventGridClientWrapper.PublishBasicEventAsync("Roberto", "netninjatest", "1.0", messages);
                
                return "Message sent successfully";
            }
            catch (Exception e)
            {
                _logger.LogError($"💀 Exception thrown by nuget is : {e.Message}");
                throw;
            }
        }

        private List<ServiceBusMessageCommand> BuildMessagesForServiceBus()
        {
            var listOfMessages = new List<ServiceBusMessageCommand>();

            var message1 = new ServiceBusMessageCommand { Message = "great day let's code", Emissor = "Robert", Receptor = "Claris" };
            var message2 = new ServiceBusMessageCommand { Message = "second message for queue", Emissor = "Ramiro", Receptor = "Roberto" };
                
            listOfMessages.Add(message1);
            listOfMessages.Add(message2);
            
            return listOfMessages;
        }
        
        private List<EventGridMessageCommand> BuildMessagesForEventGrid()
        {
            var listOfMessages = new List<EventGridMessageCommand>();

            var message1 = new EventGridMessageCommand { Message = "great day let's code", Emissor = "Robert", Receptor = "Claris" };
            var message2 = new EventGridMessageCommand { Message = "second message for queue", Emissor = "Ramiro", Receptor = "Roberto" };
                
            listOfMessages.Add(message1);
            listOfMessages.Add(message2);
            
            return listOfMessages;
        }
    }
};

