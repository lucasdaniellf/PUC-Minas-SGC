using Core.Messages.Commands;

namespace Vendas.Application.Commands
{
    public class AdicionarItemVendaCommand : CommandRequest
    {
        public string VendaId { get; set; } = null!;
        public string ProdutoId { get; set; } = null!;
        public int Quantidade { get; set; }
    }
}
