using Core.MessageBroker;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Core.Messages.Event
{
    public abstract class EventRequest : MessageRequest
    {
    }
}
