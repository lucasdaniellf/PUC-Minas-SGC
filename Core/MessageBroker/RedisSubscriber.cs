using Newtonsoft.Json;
using StackExchange.Redis;

namespace Core.MessageBroker
{
    public class RedisSubscriber : IMessageBrokerSubscriber
    {
        private readonly IConnectionMultiplexer _connection;
        private ChannelMessageQueue? Channel { get; set; }
        public string _channelName { get; private set; } = string.Empty;

        public RedisSubscriber(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }


        public IEnumerable<string> Dequeue()
        {
            
            if(Channel == null )
            {
                throw new ArgumentException("Necessário se inscrever em algum canal");
            } 

            IList<string> messages = new List<string>();
            Channel.TryGetCount(out int count);

            while (count != 0)
            {
                Channel.TryRead(out var queueMessage);
                var message = queueMessage.Message.ToString();
                if (!string.IsNullOrEmpty(message))
                {
                    messages.Add(message);
                }
                Channel.TryGetCount(out count);

                if(messages.Count >= 20)
                {
                    break;
                }
            }   

            return messages;
        }

        public void Subscribe(string queueName)
        {
            if(Channel == null)
            {
                Channel = _connection.GetSubscriber().Subscribe(new RedisChannel(queueName, RedisChannel.PatternMode.Pattern));
            }
        }

        public void Unsubscribe(string queueName)
        {
            if(Channel != null ) {
                _connection.GetSubscriber().Unsubscribe(queueName);
                Channel.Unsubscribe();
                Channel = null;
            }
            
        }
    }
}
