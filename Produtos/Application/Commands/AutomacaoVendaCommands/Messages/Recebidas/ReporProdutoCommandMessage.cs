using Core.Messages;

namespace Produtos.Application.Commands.AutomacaoVendaCommands.Messages.Recebidas
{
    public class ReporProdutoCommandMessage : MessageRequest
    {
        public ReporProdutoCommandMessage(string vendaId, IEnumerable<ProdutoVendaCommandMessage> Produtos)
        {
            this.VendaId = vendaId;
            this.Produtos = Produtos;
        }
        public string VendaId { get; private set; } = string.Empty!;
        public IEnumerable<ProdutoVendaCommandMessage> Produtos { get; private set; }
    }
}
