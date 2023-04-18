using Core.Messages.Commands;

namespace Produtos.Application.Commands.ProdutoEstoque
{
    public class ReporEstoqueProdutoCommand : CommandRequest
    {
        public IEnumerable<EstoqueProduto> Produtos { get; set; } = Enumerable.Empty<EstoqueProduto>();
    }
}
