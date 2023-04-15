using Core.Messages;

namespace Vendas.Application.Commands.Messages.Enviadas
{
    public class ReservarProdutoCommandMessage : MessageRequest
    {
        public ReservarProdutoCommandMessage(string VendaId, IEnumerable<ProdutoVendaCommandMessage> Produtos)
        {
            this.VendaId = VendaId;
            this.Produtos = Produtos;
        }

        public string VendaId { get; private set; } = null!;
        public IEnumerable<ProdutoVendaCommandMessage> Produtos { get; private set; } = null!;
    }
}
