using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class AplicarDescontoVendaCommand : CommandRequest
    {
        public string Id { get; set; } = null!;
        public int Desconto { get; set; }
    }
}
