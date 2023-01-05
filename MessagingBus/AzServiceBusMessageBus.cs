using Azure.Messaging.ServiceBus;
using Messages;
using Newtonsoft.Json;
using System.Text;

namespace MessagingBus
{
    public class AzServiceBusMessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://chasqui.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=WXyyW6Hjc1/VLMathATWqXE1fmrqbiSoeyucMaMp48c=";

        public async Task PublishMessage(IntegrationBaseMessage message, string topicName, string correlationId = null)
        {
            ServiceBusClient client;
            ServiceBusSender sender;

            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topicName);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            var jsonMessage = JsonConvert.SerializeObject(message);
            var serviceBusMessage = new ServiceBusMessage(jsonMessage)
            {
                CorrelationId = !string.IsNullOrEmpty(correlationId) ? correlationId : Guid.NewGuid().ToString()
            };

            if (!messageBatch.TryAddMessage(serviceBusMessage))
            {
                // if it is too large for the batch
                throw new Exception($"The message is too large to fit in the batch.");
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus topic
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"Messages have been published to the topic.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine($"Sent message to {sender.EntityPath}");
        }
    }
}