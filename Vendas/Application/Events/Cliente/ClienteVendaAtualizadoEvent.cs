using Core.Messages.Event;

namespace Vendas.Application.Events.Cliente
{
    public class ClienteVendaAtualizadoEvent : EventRequest
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int EstaAtivo { get; set; }


    }
}
