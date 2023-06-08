using Core.Messages.Event;
using static Clientes.Domain.Model.ClienteStatus;

namespace Clientes.Application.Events
{
    public class ClienteAtualizadoEvent : EventRequest
    {
        public ClienteAtualizadoEvent(string id, string email, ClienteStatusEnum status)
        {
            Id = id;
            Email = email;
            Status = status;
        }

        public string Id { get; }
        public string Email { get; }
        public ClienteStatusEnum Status { get; }
    }
}
