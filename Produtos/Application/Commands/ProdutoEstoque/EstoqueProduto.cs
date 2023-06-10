using System.ComponentModel.DataAnnotations;

namespace Produtos.Application.Commands.ProdutoEstoque
{
    public class EstoqueProduto
    {
        public string ProdutoId { get; private set; } = null!;

        [Range(0, int.MaxValue)]
        public int Quantidade { get; private set; }

        public EstoqueProduto(string produtoId, int quantidade)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
        }
    }
}
