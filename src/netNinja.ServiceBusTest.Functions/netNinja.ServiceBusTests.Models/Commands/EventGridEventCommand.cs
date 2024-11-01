
namespace netNinja.ServiceBusTests.Models.Commands
{
    public class EventGridEventCommand
    { 
        public string Id { get; set; }
        public string Topic { get; set; }
        public string Subject { get; set; }
        public dynamic Data { get; set; }
        public string EventType { get; set; }
        public DateTime EventTime { get; set; }
        public string DataVersion { get; set; }
    }
    
    public class SubscriptionValidationEventData
    {
        public string ValidationCode { get; set; }
    }
};


