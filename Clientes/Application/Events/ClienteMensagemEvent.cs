using Core.Messages.Event;
using static Clientes.Domain.Model.Status;

namespace Clientes.Application.Events
{
    public class ClienteMensagemEvent : EventRequest
    {
        public ClienteMensagemEvent(string id, string email, int estaAtivo)
        {
            Id = id;
            Email = email;
            EstaAtivo = estaAtivo;
        }

        public string Id { get; }
        public string Email { get; }
        public int EstaAtivo { get; }
    }
}
