using Core.Entity;

namespace Vendas.Domain.Model
{
    public class ItemVenda : IEntity
    {
        internal Venda Venda { get; private set; } = null!;
        public ProdutoVenda Produto { get; private set; } = null!;
        public int Quantidade { get; private set; } = 0;
        public decimal ValorPago { get; private set; } = 0;

        internal ItemVenda(Venda venda, ProdutoVenda produto, int quantidade, decimal preco)
        {
            Venda = venda;
            Produto = produto;
            AtualizarQuantidade(quantidade);
            AtualizarValorPago(preco);
        }

        internal static ItemVenda CriarItemVenda(Venda venda, ProdutoVenda produto, int quantidade, decimal preco)
        {
            var item = new ItemVenda(venda, produto, quantidade, preco);
            if (!item.ValidarProdutoItemVenda())
            {
                throw new VendaException($"Produto não pode ser adicionado à venda: Status: {item.Produto.EstaAtivo}; Estoque: {item.Produto.QuantidadeEstoque}");
            }
            return item;
        }

        internal void AtualizarQuantidadeItemVenda(int quantidade)
        {
            if(Venda.Status != StatusVenda.Status.PENDENTE)
            {
                throw new VendaException("Item em venda não pode ser atualizado para vendas com status diferente de PENDENTE. Status: " + Venda.Status);
            }
            AtualizarQuantidade(quantidade);
        }

        internal void AtualizarValorPago(decimal valorPago)
        {
            if(ValorPago < 0)
            {
                throw new VendaException("Valor Pago deve ser maior ou igual a 0.");
            }
            this.ValorPago = valorPago;
        }

        private void AtualizarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
            {
                throw new VendaException("Quantidade deve ser maior do que 0.");
            }
            this.Quantidade = quantidade;
        }

        internal bool ValidarProdutoItemVenda()
        {
            return this.Produto.EstaAtivo != ProdutoVenda.ProdutoStatus.INATIVO && (this.Quantidade <= this.Produto.QuantidadeEstoque);
        }
    }
}
