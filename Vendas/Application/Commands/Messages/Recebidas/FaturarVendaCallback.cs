using Core.Messages;

namespace Vendas.Application.Commands.Messages.Recebidas
{
    public class FaturarVendaCallback : MessageRequest
    {
        public FaturarVendaCallback(string VendaId, bool Sucesso)
        {
            this.VendaId = VendaId;
            this.Sucesso = Sucesso;
        }

        public string VendaId { get; private set; } = null!;
        public bool Sucesso { get; private set; } = false;

    }
}
