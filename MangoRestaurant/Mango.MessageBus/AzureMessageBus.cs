using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class AzureMessageBus : IMessageBus
    {
        // Remove to settings
        private const string ConnectionString = "Endpoint=sb://yevgeniymango.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KFgE782fsj+utgoGaDbGa/3HUdGur00Qy66KJ8rUqTg=";
        public async Task PublishMessageAsync(BaseMessage message, string topicName)
        {
            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(topicName);

            var jsonMessage = JsonConvert.SerializeObject(message);
            var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);
        }
    }
}
