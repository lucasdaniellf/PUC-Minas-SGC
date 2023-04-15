using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Channels;

namespace Core.MessageBroker
{
    public class RedisPublisher : IMessageBrokerPublisher
    {
        private readonly IConnectionMultiplexer _connection;

        public RedisPublisher(IConnectionMultiplexer connectionMultiplexer)
        {
            _connection = connectionMultiplexer;
        }

        public async Task Enqueue(string queueName, string message)
        {
            await _connection.GetSubscriber().PublishAsync(queueName, message);
        }
    }
}
