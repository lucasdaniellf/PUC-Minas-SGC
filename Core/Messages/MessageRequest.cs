using Core.MessageBroker;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Core.Messages
{
    public abstract class MessageRequest
    {
        public string MessageId { get; } = Guid.NewGuid().ToString();
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
