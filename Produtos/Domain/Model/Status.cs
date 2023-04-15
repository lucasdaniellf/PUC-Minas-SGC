namespace Produtos.Domain.Model
{
    public class Status
    {
        public enum ProdutoStatus
        {
            INATIVO = 0,
            ATIVO = 1
        }

        public static ProdutoStatus AplicarStatusEmProduto(long value)
        {
            return value == 0 ? ProdutoStatus.INATIVO : ProdutoStatus.ATIVO;
        }
    }
}
