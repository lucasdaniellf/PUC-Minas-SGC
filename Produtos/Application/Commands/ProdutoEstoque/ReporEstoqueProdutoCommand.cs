using Core.Messages.Commands;

namespace Produtos.Application.Commands.ProdutoEstoque
{
    public class ReporEstoqueProdutoCommand : CommandRequest
    {
        public IEnumerable<EstoqueProduto> Produtos { get; private set; } = Enumerable.Empty<EstoqueProduto>();
        public ReporEstoqueProdutoCommand(IEnumerable<EstoqueProduto> produtos)
        {
            Produtos = produtos;
        }
    }
}
