using Core.Messages.Commands;
using Vendas.Domain.Model;
using static Vendas.Domain.Model.FormaPagamentoEnum;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Application.Commands
{
    public class AtualizarVendaCommand : CommandRequest
    {
        public string Id { get; private set; } = null!;
        public int Desconto { get; set; }
        public FormaPagamento FormaDePagamento { get; set; }
        //public Status StatusVenda { get; set; }

        public void AdicionarId(string id)
        {
            this.Id = id;
        }
    }
}
