using Core.Messages.Event;

namespace Vendas.Application.Events.Cliente
{
    public class ClienteVendaAtualizadoEvent : EventRequest
    {
        public string Id { get; private set; } = string.Empty!;
        public string Email { get; private set; } = string.Empty;
        public int Status { get; private set; }

        public ClienteVendaAtualizadoEvent(string id, string email, int status)
        {
            Id = id;
            Email = email;
            Status = status;
        }
    }
}
