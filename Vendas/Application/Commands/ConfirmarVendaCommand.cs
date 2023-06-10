using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class ConfirmarVendaCommand : CommandRequest
    {
        public string Id { get; private set; } = null!;
        public ConfirmarVendaCommand(string id)
        {
            Id = id;
        }
    }
}
