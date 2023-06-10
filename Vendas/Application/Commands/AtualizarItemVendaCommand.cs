using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class AtualizarItemVendaCommand : CommandRequest
    {
        public string VendaId { get; private set; } = string.Empty;
        public string ProdutoId { get; private set; } = string.Empty!;
        public int Quantidade { get; private set; }

        public AtualizarItemVendaCommand(string vendaId, string produtoId, int quantidade)
        {
            VendaId = vendaId;
            ProdutoId = produtoId;
            Quantidade = quantidade;
        }
    }
}
