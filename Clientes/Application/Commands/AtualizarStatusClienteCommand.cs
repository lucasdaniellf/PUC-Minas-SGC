using Core.Messages.Commands;
using static Clientes.Domain.Model.ClienteStatus;

namespace Clientes.Application.Commands
{
    public class AtualizarStatusClienteCommand : CommandRequest
    {
        public string Id { get; set; } = null!;
        public ClienteStatusEnum Status { get; set; }

        public AtualizarStatusClienteCommand(string id, ClienteStatusEnum status)
        {
            Id = id;
            Status = status;
        }
    }
}
