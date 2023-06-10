using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class AplicarDescontoVendaCommand : CommandRequest
    {
        public string Id { get; private set; } = string.Empty;
        public int Desconto { get; set; }

        public AplicarDescontoVendaCommand(string id, int desconto)
        {
            Id = id;
            Desconto = desconto;
        }
    }
}
