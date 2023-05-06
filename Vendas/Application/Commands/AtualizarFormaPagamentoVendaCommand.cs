using Core.Messages.Commands;
using static Vendas.Domain.Model.FormaPagamentoEnum;

namespace Vendas.Application.Commands
{
    public class AtualizarFormaPagamentoVendaCommand : CommandRequest
    {
        public string Id { get; private set; } = string.Empty;
        public FormaPagamento FormaDePagamento { get; set; }

        public void AdicionarId(string id)
        {
            this.Id = id;
        }
    }
}
