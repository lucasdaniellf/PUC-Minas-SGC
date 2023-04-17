using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class ConfirmarVendaCommand : CommandRequest
    {
        public string Id { get; init; } = null!;
        public ConfirmarVendaCommand(string id)
        {
            Id = id;
        }
    }
}
