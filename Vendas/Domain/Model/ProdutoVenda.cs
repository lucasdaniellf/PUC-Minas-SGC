using Core.Entity;

namespace Vendas.Domain.Model
{
    public class ProdutoVenda : IEntity
    {
        public string Id { get; private set; } = null!;
        public decimal Preco { get; private set; }
        public int QuantidadeEstoque { get; private set; }
        public ProdutoStatus Status { get; private set; }

        private ProdutoVenda() { }

        internal ProdutoVenda(string id, decimal preco, int quantidadeEstoque, int ativo)
        {
            Id = id;
            Preco = preco;
            AplicarStatusEmProduto(ativo);
            QuantidadeEstoque = quantidadeEstoque;
        }


        public enum ProdutoStatus
        {
            INATIVO = 0,
            ATIVO = 1
        }

        private void AplicarStatusEmProduto(int value)
        {
            this.Status = value == 0 ? ProdutoStatus.INATIVO : ProdutoStatus.ATIVO;
        }
    }
}
