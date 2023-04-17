using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class AdicionarItemVendaCommand : CommandRequest
    {
        public string VendaId { get; private set; } = string.Empty;
        public string ProdutoId { get; set; } = null!;
        public int Quantidade { get; set; }

        public void AdicionarVendaId(string vendaId)
        {
            VendaId = vendaId;
        }
    }
}
