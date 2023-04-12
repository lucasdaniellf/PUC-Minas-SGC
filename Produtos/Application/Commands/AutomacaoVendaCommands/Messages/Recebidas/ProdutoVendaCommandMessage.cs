using Core.Messages;

namespace Produtos.Application.Commands.AutomacaoVendaCommands.Messages.Recebidas
{
    public class ProdutoVendaCommandMessage : MessageRequest
    {
        public ProdutoVendaCommandMessage(string ProdutoId, int Quantidade)
        {
            this.ProdutoId = ProdutoId;
            this.Quantidade = Quantidade;
        }

        public string ProdutoId { get; private set; } = null!;
        public int Quantidade { get; private set; }
    }
}
