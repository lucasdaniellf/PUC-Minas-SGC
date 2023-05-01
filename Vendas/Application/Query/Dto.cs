using static Vendas.Domain.Model.ClienteVenda;
using static Vendas.Domain.Model.FormaPagamentoEnum;
using static Vendas.Domain.Model.ProdutoVenda;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Application.Query
{
    public record VendaDto(string id, ClienteDto cliente, DateTime dataVenda, int desconto, Status status, FormaPagamento formaPagamento, string criadoPor, IEnumerable<VendaItemDto> itens);
    public record VendaItemDto(string vendaId, string produtoId, decimal valorPago, int quantidade);
    public record ClienteDto(string id, string Email, int status);
    public record ProdutoDto(string id, decimal preco, int estoque, int status);
}
