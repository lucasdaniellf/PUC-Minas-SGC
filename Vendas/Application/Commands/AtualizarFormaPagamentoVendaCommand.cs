using Core.Messages.Commands;
using static Vendas.Domain.Model.FormaPagamentoEnum;

namespace Vendas.Application.Commands
{
    public class AtualizarFormaPagamentoVendaCommand : CommandRequest
    {
        public string Id { get; private set; } = string.Empty;
        public FormaPagamento FormaDePagamento { get; private set; }

        public AtualizarFormaPagamentoVendaCommand(string id, FormaPagamento formaPagamento)
        {
            Id = id;
            FormaDePagamento = formaPagamento;
        }
    }
}
