using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class RemoverItemVendaCommand : CommandRequest
    {
        public string VendaId { get; private set; } = string.Empty;
        public string ProdutoId { get; private set; } = string.Empty;

        public RemoverItemVendaCommand(string VendaId, string ProdutoId)
        {
            this.VendaId = VendaId;
            this.ProdutoId = ProdutoId;
        }
    }
}
