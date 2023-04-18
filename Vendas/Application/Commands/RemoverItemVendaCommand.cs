using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class RemoverItemVendaCommand : CommandRequest
    {
        public string VendaId { get; private set; } = string.Empty;
        public string ProdutoId { get; set; } = null!;

        public void AdicionarVendaId(string vendaId)
        {
            VendaId = vendaId;
        }
    }
}
