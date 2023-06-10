using Core.Messages.Commands;

namespace Produtos.Application.Commands.ProdutoEstoque
{
    public class BaixarEstoqueProdutoCommand : CommandRequest
    {
        public IEnumerable<EstoqueProduto> Produtos { get; private set; } = Enumerable.Empty<EstoqueProduto>();
        public BaixarEstoqueProdutoCommand(IEnumerable<EstoqueProduto> produtos)
        {
            Produtos = produtos;
        }
    }
}
