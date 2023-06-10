using Vendas.Domain.Model;
using static Vendas.Domain.Model.FormaPagamentoEnum;

namespace AplicacaoGerenciamentoLoja.Controllers.Vendas.Parametros
{
    public record CriarVendaIntRequest(string ClienteId);
    public record AplicarDescontoIntRequest(string VendaId, int Desconto);
    public record AlterarFormaPagamentoIntRequest(string VendaId, FormaPagamento FormaPagamento);
}
