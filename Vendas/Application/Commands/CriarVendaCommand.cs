using Core.Messages.Commands;
namespace Vendas.Application.Commands
{
    public class CriarVendaCommand : CommandRequest
    {
        public string Id { get; internal set; } = string.Empty;
        public string ClienteId { get; private set; } = string.Empty;
        public string CriadoPor { get; private set; } = string.Empty;

        public CriarVendaCommand(string clienteId, string criadoPor)
        {
            ClienteId = clienteId;
            CriadoPor = criadoPor;
        }
    }
}
