using Core.Messages.Commands;
using static Clientes.Domain.Model.Status;

namespace Clientes.Application.Commands
{
    public class AtualizarStatusClienteCommand : CommandRequest
    {
        public string Id { get; private set; } = string.Empty;
        public ClienteStatus EstaAtivo { get; set; }

        public void AdicionarId(string id)
        {
            Id = id;
        }
    }
}
