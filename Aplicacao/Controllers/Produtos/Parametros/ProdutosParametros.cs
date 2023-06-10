using static Produtos.Domain.Model.ProdutoStatus;

namespace AplicacaoGerenciamentoLoja.Controllers.Produtos.Parametros
{
    public record CadastrarProdutoRequest(string Descricao, decimal Preco);
    public record AtualizarProdutoRequest(string Descricao, decimal Preco, int EstoqueMinimo, ProdutoStatusEnum Status);
    public record AtualizarEstoqueRequest(string ProdutoId, int Quantidade);
}
