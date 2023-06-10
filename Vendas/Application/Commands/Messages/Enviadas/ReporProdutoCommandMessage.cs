using Core.Messages;

namespace Vendas.Application.Commands.Messages.Enviadas
{
    public class ReporProdutoCommandMessage : MessageRequest
    {
        public ReporProdutoCommandMessage(string vendaId, IEnumerable<ProdutoVendaCommandMessage> Produtos)
        {
            this.Produtos = Produtos;
            this.VendaId = vendaId;
        }
        public string VendaId { get; private set; } = string.Empty;
        public IEnumerable<ProdutoVendaCommandMessage> Produtos { get; private set; }
    }
}
