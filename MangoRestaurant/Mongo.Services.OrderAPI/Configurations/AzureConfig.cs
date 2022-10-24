namespace Mongo.Services.OrderAPI.Configurations
{
    public class AzureConfig
    {
        public string ConnectionString { get; set; }
        public string CheckOutMessageTopic { get; set; }
        public string CheckOutQueue { get; set; }
        public string SubscriptionCheckout { get; set; }
        public string OrderPaymentProcessTopic { get; set; }
        public string MangoPaymentSubscription { get; set; }
        public string OrderUpdatePaymentResultTopic { get; set; }
    }
}
