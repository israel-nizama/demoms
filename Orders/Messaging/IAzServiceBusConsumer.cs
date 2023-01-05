namespace Orders.Messaging
{
    public interface IAzServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
