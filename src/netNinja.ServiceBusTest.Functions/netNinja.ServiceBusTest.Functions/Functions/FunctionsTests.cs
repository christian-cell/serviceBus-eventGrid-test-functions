using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using netNinja.ServiceBusTest.Services.Abstractions;
using netNinja.ServiceBusTests.Models.Commands;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace netNinja.ServiceBusTest.Functions.Functions
{
    public class FunctionsTests
    {
        private readonly ILogger<FunctionsTests> _logger;
        private readonly IServiceBusTestSendService _serviceBusTestSendService;

        public FunctionsTests(
            ILogger<FunctionsTests> logger,
            IServiceBusTestSendService serviceBusTestSendService
        )
        {
            _logger = logger;
            _serviceBusTestSendService = serviceBusTestSendService;
        }
        
        [Function("EventGridWebHook")]
        [AllowAnonymous]
        [OpenApiOperation(operationId: "ReceiveEventGridEvent", tags: new[] { "user" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(EventGridEventCommand), Required = true, Description = "the minimum message data to send")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Task<ActionResult>), Description = "The response")]
        public  HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,FunctionContext context)
        {
            _logger.LogInformation($"estamos dentro del webhook");

            string requestBody = req.ReadAsStringAsync().Result;
            
            var eventDataArray = JsonConvert.DeserializeObject<EventGridEventCommand[]>(requestBody);
            
            var response = req.CreateResponse(HttpStatusCode.OK);

            foreach (var eventData in eventDataArray)
            {
                switch (eventData.EventType)
                {
                    case "Microsoft.EventGrid.SubscriptionValidationEvent":
                        var validationData = JsonConvert.DeserializeObject<SubscriptionValidationEventData>(eventData.Data.ToString());
                        _logger.LogInformation($"Subject: {eventData.Subject}");
                        _logger.LogInformation($"ValidationCode: {validationData.ValidationCode}");
                        response.WriteAsJsonAsync(new { validationResponse = validationData.ValidationCode });
                        break;

                    default:
                        _logger.LogInformation($"Event Type: {eventData.EventType}");
                        _logger.LogInformation($"Event Data: {eventData.Data.ToString()}");
                        break;
                }
            }

            return response;
        }
        
        /// <summary>
        /// Send message to Eventgrid or ServiceBus
        /// </summary>
        /// <param name="req"> The HTTP request data.</param>
        /// <returns> response will be a simply ActionResult </return>
        [Function("send-SB-message")]
        [AllowAnonymous]
        [OpenApiOperation(operationId: "SendMessageToEventGridOrServiceBus", tags: new[] { "user" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ServiceBusMessageCommand), Required = true, Description = "the minimum message data to send")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Task<ActionResult>), Description = "The response")]
        public async Task<ActionResult> SendToServiceBusAsync([Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                StreamReader reader = new StreamReader(req.Body);

                ServiceBusMessageCommand messageCommand = JsonSerializer.Deserialize<ServiceBusMessageCommand>(reader.ReadToEnd(), options) ?? new ServiceBusMessageCommand();

                if (messageCommand == null) throw new Exception("message can't be null");

                _logger.LogInformation($"... lets create user {messageCommand.Message}");

                var result = await _serviceBusTestSendService.SendToServiceBusAsync(messageCommand);

                var eventgridSasToken = "sadf";
                
                return new OkObjectResult(new { result = result });
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"something went wrong creating new user {ex}");
                
                throw new Exception($"something went wrong creating new user {ex}");
            }
            
        }
        
        /// <summary>
        /// Send message to Eventgrid 
        /// </summary>
        /// <param name="req"> The HTTP request data.</param>
        /// <returns> response will be a simply ActionResult </return>
        [Function("send-EG-event")]
        [AllowAnonymous]
        [OpenApiOperation(operationId: "SendMessageToEventGridOrServiceBus", tags: new[] { "user" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ServiceBusMessageCommand), Required = true, Description = "the minimum message data to send")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Task<ActionResult>), Description = "The response")]
        public async Task<ActionResult> SendEventToEventGridAsync([Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                StreamReader reader = new StreamReader(req.Body);

                EventGridMessageCommand messageCommand = JsonSerializer.Deserialize<EventGridMessageCommand>(reader.ReadToEnd(), options) ?? new EventGridMessageCommand();

                if (messageCommand == null) throw new Exception("message can't be null");

                _logger.LogInformation($"... lets create user {messageCommand.Message}");

                var result = await _serviceBusTestSendService.SendToEventToEventGrid(messageCommand);

                var eventgridSasToken = "sadf";
                
                return new OkObjectResult(new { result = result });
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"something went wrong creating new user {ex}");
                
                throw new Exception($"something went wrong creating new user {ex}");
            }
            
        }
    }
};

