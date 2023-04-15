using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class RemoverItemVendaCommand : CommandRequest
    {
        public string VendaId { get; set; } = null!;
        public string ProdutoId { get; set; } = null!;
    }
}
