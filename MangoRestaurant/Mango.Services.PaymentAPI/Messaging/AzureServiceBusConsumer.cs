using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Configurations;
using Microsoft.Extensions.Options;
using Mongo.Services.PaymentAPI.Messages;

using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mongo.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly AzureConfig _azureConfig;
        private ServiceBusProcessor orderPaymentProcessor;
        private readonly IProcessPayment _processPayment;
        private readonly IMessageBus _messageBus;

        public AzureServiceBusConsumer(IProcessPayment processPayment, IOptions<AzureConfig> azureConfig, IMessageBus messageBus)
        {
            _azureConfig = azureConfig.Value;
            _messageBus = messageBus;
            _processPayment = processPayment;
            var client = new ServiceBusClient(_azureConfig.ConnectionString);
            orderPaymentProcessor = client.CreateProcessor(_azureConfig.OrderPaymentProcessTopic, _azureConfig.MangoPaymentSubscription);            
        }

        public async Task Start()
        {
            orderPaymentProcessor.ProcessMessageAsync += ProcessPayment;
            orderPaymentProcessor.ProcessErrorAsync += (args) =>
            {
                return Task.CompletedTask;
            };

            await orderPaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await orderPaymentProcessor.StopProcessingAsync();
            await orderPaymentProcessor.DisposeAsync();

        }
        private async Task ProcessPayment(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                OrderId = paymentRequestMessage.OrderId,
                Status = result,
                Email = paymentRequestMessage.Email,
            };            

            try
            {
                await _messageBus.PublishMessageAsync(updatePaymentResultMessage, _azureConfig.OrderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
