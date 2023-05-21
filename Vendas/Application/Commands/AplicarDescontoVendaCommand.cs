using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class AplicarDescontoVendaCommand : CommandRequest
    {
        public string Id { get; private set; } = null!;
        public int Desconto { get; set; }

        public void InformarId(string id)
        {
            Id = id;
        }
    }
}
