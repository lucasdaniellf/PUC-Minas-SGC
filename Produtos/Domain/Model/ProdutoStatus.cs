namespace Produtos.Domain.Model
{
    public class ProdutoStatus
    {
        public enum ProdutoStatusEnum
        {
            INATIVO = 0,
            ATIVO = 1
        }

        public static ProdutoStatusEnum AplicarStatusEmProduto(long value)
        {
            return value == 0 ? ProdutoStatusEnum.INATIVO : ProdutoStatusEnum.ATIVO;
        }
    }
}
