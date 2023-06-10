using Core.Messages.Commands;

namespace Vendas.Application.Commands.AutomacaoVendaCommands
{
    public class AprovarVendaCommand : CommandRequest
    {
        public AprovarVendaCommand(string vendaId)
        {
            VendaId = vendaId;
        }

        public string VendaId { get; set; } = null!;
    }
}
