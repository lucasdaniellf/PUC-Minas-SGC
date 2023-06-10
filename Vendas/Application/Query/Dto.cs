using static Vendas.Domain.Model.ClienteVenda;
using static Vendas.Domain.Model.FormaPagamentoEnum;
using static Vendas.Domain.Model.ProdutoVenda;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Application.Query
{
    public record VendaDto(string Id, ClienteDto Cliente, DateTime DataVenda, int Desconto, Status Status, FormaPagamento FormaPagamento, string CriadoPor, IEnumerable<VendaItemDto> Itens);
    public record VendaItemDto(string ProdutoId, decimal ValorPago, int Quantidade);
    public record ClienteDto(string Id, string Email, ClienteStatus Status);
    public record ProdutoDto(string Id, decimal Preco, int Estoque, ProdutoStatus Status);
}
