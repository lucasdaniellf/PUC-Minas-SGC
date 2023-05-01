using Core.Messages.Commands;
namespace Vendas.Application.Commands
{
    public class CriarVendaCommand : CommandRequest
    {
        public string Id { get; internal set; } = string.Empty;
        public string ClienteId { get; set; } = null!;
        public string CriadoPor { get; private set; } = string.Empty;

        public void DefinirSolicitanteDaRequisicao (string criadoPor)
        {
            this.CriadoPor = criadoPor;
        }
    }
}
