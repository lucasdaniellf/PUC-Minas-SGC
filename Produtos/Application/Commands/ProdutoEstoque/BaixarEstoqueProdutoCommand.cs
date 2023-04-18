using Core.Messages.Commands;

namespace Produtos.Application.Commands.ProdutoEstoque
{
    public class BaixarEstoqueProdutoCommand : CommandRequest
    {
        public IEnumerable<EstoqueProduto> Produtos { get; set; } = Enumerable.Empty<EstoqueProduto>();
    }
}
