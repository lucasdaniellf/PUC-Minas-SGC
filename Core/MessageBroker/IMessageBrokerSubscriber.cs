using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MessageBroker
{
    public interface IMessageBrokerSubscriber
    {
        public void Subscribe(string queueName);
        public void Unsubscribe(string queueName);
        public IEnumerable<string> Dequeue();
    }
}
