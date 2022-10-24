namespace Mango.Services.ShoppingCartAPI.Configurations
{
    public class ServiceBusConfig
    {
        public string CheckoutTopicName { get; set; }
        public string CheckoutQueueName { get; set; }
    }
}
