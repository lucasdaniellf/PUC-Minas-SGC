using Produtos.Domain.Model;

namespace Produtos.Infrastructure
{
    internal record ProdutoTO(string Id, string Descricao, double Preco, long EstaAtivo, string EstoqueId, long Quantidade, long EstoqueMinimo, string UltimaAlteracao);

    internal class ProdutoRepositoryMapping
    {
        internal static Produto MapearProdutoEstoque(ProdutoTO to)
        {
            return new Produto(to.Id, to.Descricao, (decimal)to.Preco, to.EstaAtivo, to.EstoqueId, (int)to.Quantidade, (int)to.EstoqueMinimo, Convert.ToDateTime(to.UltimaAlteracao));
        }
    }
}
