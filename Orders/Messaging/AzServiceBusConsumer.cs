using Azure.Messaging.ServiceBus;
using MessagingBus;
using Newtonsoft.Json;
using Orders.Entities;
using Orders.Messages;
using Orders.Repositories;
using System.Diagnostics;
using System.Text;

namespace Orders.Messaging
{
    public class AzServiceBusConsumer : IAzServiceBusConsumer
    {
        private readonly string subscriptionName = "ordersubscription";
        ServiceBusClient client;
        private readonly ServiceBusProcessor checkoutMessageReceiverClient;
        private readonly ServiceBusProcessor orderPaymentUpdateMessageReceiverClient;

        private readonly IConfiguration _configuration;

        private readonly OrderRepository _orderRepository;
        private readonly ILogger<AzServiceBusConsumer> logger;
        private readonly IMessageBus _messageBus;

        private readonly string checkoutMessageTopic;
        private readonly string orderPaymentRequestMessageTopic;
        private readonly string orderPaymentUpdatedMessageTopic;

        public AzServiceBusConsumer(IConfiguration configuration, OrderRepository orderRepository, ILogger<AzServiceBusConsumer> logger, IMessageBus messageBus)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
            this.logger = logger;
            _messageBus = messageBus;

            var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            orderPaymentRequestMessageTopic = _configuration.GetValue<string>("OrderPaymentRequestMessageTopic");
            orderPaymentUpdatedMessageTopic = _configuration.GetValue<string>("OrderPaymentUpdatedMessageTopic");

            client = new ServiceBusClient(serviceBusConnectionString);
            checkoutMessageReceiverClient = client.CreateProcessor(checkoutMessageTopic, subscriptionName, new ServiceBusProcessorOptions());
            orderPaymentUpdateMessageReceiverClient = client.CreateProcessor(orderPaymentUpdatedMessageTopic, subscriptionName, new ServiceBusProcessorOptions());
        }

        public async Task Start()
        {
            try
            {
                // add handler to process messages
                checkoutMessageReceiverClient.ProcessMessageAsync += OnCheckoutMessageReceived;
                orderPaymentUpdateMessageReceiverClient.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
                // add handler to process any errors
                checkoutMessageReceiverClient.ProcessErrorAsync += OnServiceBusException;
                orderPaymentUpdateMessageReceiverClient.ProcessErrorAsync += OnServiceBusException;
                // start processing 
                await checkoutMessageReceiverClient.StartProcessingAsync();
                await orderPaymentUpdateMessageReceiverClient.StartProcessingAsync();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Unable to publish checkout message");
                await checkoutMessageReceiverClient.DisposeAsync();
                await orderPaymentUpdateMessageReceiverClient.DisposeAsync();
                await client.DisposeAsync();
                throw;
            }
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var body = Encoding.UTF8.GetString(args.Message.Body);//json from service bus
            Console.WriteLine($"Received: {body} from subscription.");
            OrderPaymentUpdateMessage orderPaymentUpdateMessage =
                JsonConvert.DeserializeObject<OrderPaymentUpdateMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(orderPaymentUpdateMessage.OrderId, orderPaymentUpdateMessage.PaymentSuccess);
            // complete the message. messages is deleted from the subscription. 
            await args.CompleteMessageAsync(args.Message);
        }

        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            using var scope = logger.BeginScope("Processing message for trace {TraceId}", args.Message.CorrelationId);

            var body = Encoding.UTF8.GetString(args.Message.Body);//json from service bus
            Console.WriteLine($"Received: {body} from subscription.");

            //save order with status not paid
            BasketCheckoutMessage basketCheckoutMessage = JsonConvert.DeserializeObject<BasketCheckoutMessage>(body);

            Guid orderId = Guid.NewGuid();

            Order order = new Order
            {
                UserId = basketCheckoutMessage.UserId,
                Id = orderId,
                OrderPaid = false,
                OrderPlaced = DateTime.Now,
                OrderTotal = basketCheckoutMessage.BasketTotal
            };

            await _orderRepository.AddOrder(order);

            logger.LogDebug("Created order {OrderId} for user {UserId}", orderId, basketCheckoutMessage.UserId);

            //send order payment request message
            OrderPaymentRequestMessage orderPaymentRequestMessage = new OrderPaymentRequestMessage
            {
                CardExpiration = basketCheckoutMessage.CardExpiration,
                CardName = basketCheckoutMessage.CardName,
                CardNumber = basketCheckoutMessage.CardNumber,
                OrderId = orderId,
                Total = basketCheckoutMessage.BasketTotal
            };

            try
            {
                await _messageBus.PublishMessage(orderPaymentRequestMessage, orderPaymentRequestMessageTopic, args.Message.CorrelationId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // complete the message. messages is deleted from the subscription. 
            await args.CompleteMessageAsync(args.Message);
        }

        private Task OnServiceBusException(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());

            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            try
            {
                Console.WriteLine("\nStopping the receiver...");
                await checkoutMessageReceiverClient.StopProcessingAsync();
                await orderPaymentUpdateMessageReceiverClient.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Unable to publish checkout message");
                await checkoutMessageReceiverClient.DisposeAsync();
                await orderPaymentUpdateMessageReceiverClient.DisposeAsync();
                await client.DisposeAsync();
                throw;
            }
        }
    }
}
