using Core.Messages.Commands;
using static Clientes.Domain.Model.Status;

namespace Clientes.Application.Commands
{
    public class AtualizarStatusClienteCommand : CommandRequest
    {
        public string Id { get; set; } = null!;
        public ClienteStatus EstaAtivo { get; set; }

    }
}
