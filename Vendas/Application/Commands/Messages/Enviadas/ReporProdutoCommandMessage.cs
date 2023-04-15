using Core.Messages;

namespace Vendas.Application.Commands.Messages.Enviadas
{
    public class ReporProdutoCommandMessage : MessageRequest
    {
        public ReporProdutoCommandMessage(IEnumerable<ProdutoVendaCommandMessage> Produtos)
        {
            this.Produtos = Produtos;
        }

        public IEnumerable<ProdutoVendaCommandMessage> Produtos { get; private set; }
    }
}
