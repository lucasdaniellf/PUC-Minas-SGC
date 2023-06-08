using static Produtos.Domain.Model.ProdutoStatus;

namespace Produtos.Application.Query.DTO
{
    public record ProdutoQueryDto(string Id, string Descricao, decimal Preco, EstoqueDto Estoque, ProdutoStatusEnum Status);
    public record EstoqueDto(int Quantidade, int EstoqueMinimo);
}
