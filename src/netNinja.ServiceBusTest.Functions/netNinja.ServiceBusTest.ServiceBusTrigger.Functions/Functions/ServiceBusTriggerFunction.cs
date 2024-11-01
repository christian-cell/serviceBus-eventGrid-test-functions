using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using netNinja.ServiceBusTests.Models.Commands;
using Newtonsoft.Json;

namespace netNinja.ServiceBusTest.ServiceBusTrigger.Functions.Functions
{
    public class ServiceBusTriggerFunction
    {
        private readonly ILogger<ServiceBusTriggerFunction> _logger;

        public ServiceBusTriggerFunction(
            ILogger<ServiceBusTriggerFunction> logger
        )
        {
            _logger = logger;
        }
        
        [Function("ProcessServiceBusQueueMessage")]
        public  void Run(
            [ServiceBusTrigger("netninjaservicebusqueuetests", Connection = "SERVICEBUS_CONNECTION_STRING")]
            string message)
        {
            _logger.LogInformation($"C# ServiceBus queue trigger function processed message: {message}");
            
            /*TODO fix this error in the following line*/
            /*Result: Function 'ProcessServiceBusQueueMessage', Invocation id '30fd34f1-4879-4aae-9a51-de5e6448eb56': An exception was thrown by the invocation.
                Exception: Newtonsoft.Json.JsonSerializationException: Cannot deserialize the current JSON object (e.g. {"name":"value"}) into type 'System.Collections.Generic.List`1[netNinja.ServiceBusTests.Models.Commands.ServiceBusMessageCommand]' because the type requires a JSON array (e.g. [1,2,3]) to deserialize correctly.
            */
            
            //var messageDeserialized = JsonConvert.DeserializeObject<List<ServiceBusMessageCommand>>(message);
            
            //_logger.LogInformation($"{messageDeserialized}");
        }
    }
};

