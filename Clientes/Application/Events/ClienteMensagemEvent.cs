using Core.Messages.Event;
using static Clientes.Domain.Model.Status;

namespace Clientes.Application.Events
{
    public class ClienteMensagemEvent : EventRequest
    {
        public ClienteMensagemEvent(string id, int estaAtivo)
        {
            Id = id;
            EstaAtivo = estaAtivo;
        }

        public string Id { get; }
        public int EstaAtivo { get; }
    }
}
