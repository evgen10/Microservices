namespace Mango.Services.PaymentAPI.Configurations
{
    public class AzureConfig
    {
        public string ConnectionString { get; set; }
        public string OrderPaymentProcessTopic { get; set; }
        public string MangoPaymentSubscription { get; set; }

        public string OrderUpdatePaymentResultTopic {get;set;}
    }
}
