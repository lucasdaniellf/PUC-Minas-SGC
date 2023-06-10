using Core.Messages.Commands;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Application.Commands
{
    public class AtualizarStatusVendaCommand : CommandRequest
    {
        public string Id { get; private set; } = null!;
        public Status Status { get; private set; }

        public AtualizarStatusVendaCommand(string id, Status status)
        {
            Id = id;
            Status = status;
        }
    }
}
