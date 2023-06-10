using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class AdicionarItemVendaCommand : CommandRequest
    {
        public string VendaId { get; private set; } = string.Empty;
        public string ProdutoId { get; private set; } = string.Empty;
        public int Quantidade { get; private set; }

        public AdicionarItemVendaCommand(string VendaId, string ProdutoId, int quantidade)
        {
            this.VendaId = VendaId;
            this.ProdutoId = ProdutoId;
            this.Quantidade = quantidade;
        }
    }
}
