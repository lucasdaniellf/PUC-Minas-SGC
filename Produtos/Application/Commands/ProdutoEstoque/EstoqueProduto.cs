using System.ComponentModel.DataAnnotations;

namespace Produtos.Application.Commands.ProdutoEstoque
{
    public class EstoqueProduto
    {
        public string ProdutoId { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int Quantidade { get; set; }
    }
}
