using Core.Messages.Commands;

namespace Vendas.Application.Commands.AutomacaoVendaCommands
{
    public class ReprovarVendaCommand : CommandRequest
    {
        public ReprovarVendaCommand(string vendaId)
        {
            VendaId = vendaId;
        }

        public string VendaId { get; set; } = null!;
    }
}
