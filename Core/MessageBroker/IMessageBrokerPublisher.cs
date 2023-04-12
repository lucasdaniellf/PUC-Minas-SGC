namespace Core.MessageBroker
{
    public interface IMessageBrokerPublisher
    {
        public Task Enqueue(string queueName, string message);
    }
}
