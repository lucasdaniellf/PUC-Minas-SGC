using Core.Messages.Commands;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Application.Commands
{
    public class AtualizarStatusVendaCommand : CommandRequest
    {
        public string Id { get; set; } = null!;
        public Status Status { get; set; }
    }
}
