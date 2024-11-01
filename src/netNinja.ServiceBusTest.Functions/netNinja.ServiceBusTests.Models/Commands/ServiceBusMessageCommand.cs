namespace netNinja.ServiceBusTests.Models.Commands
{
    public class ServiceBusMessageCommand
    {
        public string Message { get; set; }
        public string Emissor { get; set; }
        public string Receptor { get; set; }
    }
};

