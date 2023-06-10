using Core.Messages;

namespace Produtos.Application.Commands.AutomacaoVendaCommands.Messages.Recebidas
{
    public class ReservarProdutoCommandMessage : MessageRequest
    {
        public ReservarProdutoCommandMessage(string VendaId, IEnumerable<ProdutoVendaCommandMessage> Produtos)
        {
            this.VendaId = VendaId;
            this.Produtos = Produtos;
        }

        public string VendaId { get; private set; } = string.Empty!;
        public IEnumerable<ProdutoVendaCommandMessage> Produtos { get; private set; } = null!;
    }
}
