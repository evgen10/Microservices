using AutoMapper;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Microsoft.Extensions.Options;
using Mongo.Services.OrderAPI.Configurations;
using Mongo.Services.OrderAPI.Messages;
using Mongo.Services.OrderAPI.Models;
using Mongo.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Mongo.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly AzureConfig _azureConfig;
        private ServiceBusProcessor checkoutProcessor;
        private ServiceBusProcessor orderUpdatePaymentResultProcessor;
        private readonly IMessageBus _messageBus;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IMapper mapper, IOptions<AzureConfig> azureConfig, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _azureConfig = azureConfig.Value;
            _messageBus = messageBus;

            var client = new ServiceBusClient(_azureConfig.ConnectionString);
            checkoutProcessor = client.CreateProcessor(_azureConfig.CheckOutQueue);
            orderUpdatePaymentResultProcessor = client.CreateProcessor(_azureConfig.OrderUpdatePaymentResultTopic, _azureConfig.MangoPaymentSubscription);
        }

        public async Task Start()
        {
            checkoutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkoutProcessor.ProcessErrorAsync += (args) =>
            {
                return Task.CompletedTask;
            };            

            await checkoutProcessor.StartProcessingAsync();

            orderUpdatePaymentResultProcessor.ProcessMessageAsync += OnOrederPaymentUpdateReceived;
            orderUpdatePaymentResultProcessor.ProcessErrorAsync += (args) =>
            {
                return Task.CompletedTask;
            };            

            await orderUpdatePaymentResultProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await checkoutProcessor.StopProcessingAsync();
            await checkoutProcessor.DisposeAsync();

            await orderUpdatePaymentResultProcessor.StopProcessingAsync();
            await orderUpdatePaymentResultProcessor.DisposeAsync();

        }
        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = _mapper.Map<OrderHeader>(checkoutHeaderDto);

            foreach (var item in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = _mapper.Map<OrderDetails>(item);
                orderHeader.CartTotalItems += orderDetails.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            await _orderRepository.AddOrderAsync(orderHeader);

            PaymentRequestMessage paymentRequestMessage = new()
            {
                Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId = orderHeader.OrderHeaderId,
                OrderTotal = orderHeader.OrderTotal,
                Email = orderHeader.Email
            };

            try
            {
                await _messageBus.PublishMessageAsync(paymentRequestMessage, _azureConfig.OrderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task OnOrederPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
