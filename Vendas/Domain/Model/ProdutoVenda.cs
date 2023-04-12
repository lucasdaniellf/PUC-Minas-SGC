using Core.Entity;

namespace Vendas.Domain.Model
{
    public class ProdutoVenda : IEntity
    {
        internal string Id { get; private set; } = null!;
        internal decimal Preco { get; private set; }
        internal int QuantidadeEstoque { get; private set; }
        internal ProdutoStatus EstaAtivo { get; private set; }

        private ProdutoVenda() { }

        internal ProdutoVenda(string id, decimal preco, int quantidadeEstoque, int ativo)
        {
            Id = id;
            Preco = preco;
            AplicarStatusEmProduto(ativo);
            QuantidadeEstoque = quantidadeEstoque;
        }


        internal enum ProdutoStatus
        {
            INATIVO = 0,
            ATIVO = 1
        }

        private void AplicarStatusEmProduto(int value)
        {
            this.EstaAtivo = value == 0 ? ProdutoStatus.INATIVO : ProdutoStatus.ATIVO;
        }
    }
}
