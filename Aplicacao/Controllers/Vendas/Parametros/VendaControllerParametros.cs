namespace AplicacaoGerenciamentoLoja.Controllers.Vendas.Parametros
{
    public record AdicionarItemEmVendaRequest(string ProdutoId, int Quantidade);
    public record AtualizarItemEmVendaRequest(string ProdutoId, int Quantidade);

}
