using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Configurations;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly EmailRepository _emailRepository;
        private readonly AzureConfig _azureConfig;
        private ServiceBusProcessor orderUpdatePaymentResultProcessor;

        public AzureServiceBusConsumer(EmailRepository emailRepository, IOptions<AzureConfig> azureConfig)
        {
            _emailRepository = emailRepository;
            _azureConfig = azureConfig.Value;
            

            var client = new ServiceBusClient(_azureConfig.ConnectionString);
            orderUpdatePaymentResultProcessor = client.CreateProcessor(_azureConfig.OrderUpdatePaymentResultTopic, _azureConfig.SubscriptionName);
        }

        public async Task Start()
        {
            orderUpdatePaymentResultProcessor.ProcessMessageAsync += OnOrederPaymentUpdateReceived;
            orderUpdatePaymentResultProcessor.ProcessErrorAsync += (args) =>
            {
                return Task.CompletedTask;
            };

            await orderUpdatePaymentResultProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await orderUpdatePaymentResultProcessor.StopProcessingAsync();
            await orderUpdatePaymentResultProcessor.DisposeAsync();
        }
       

        private async Task OnOrederPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var messageObj = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            try
            {
                await _emailRepository.SendAndLogEmailAsync(messageObj);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
