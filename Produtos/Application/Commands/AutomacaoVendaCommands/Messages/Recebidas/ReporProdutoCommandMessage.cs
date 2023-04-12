using Core.Messages;

namespace Produtos.Application.Commands.AutomacaoVendaCommands.Messages.Recebidas
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
