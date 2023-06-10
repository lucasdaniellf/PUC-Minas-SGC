using Core.Messages.Commands;

namespace Vendas.Application.Commands.AutomacaoVendaCommands
{
    public class FaturarVendaCommand : CommandRequest
    {
        public FaturarVendaCommand(string vendaId)
        {
            VendaId = vendaId;
        }

        public string VendaId { get; set; } = null!;
    }
}
