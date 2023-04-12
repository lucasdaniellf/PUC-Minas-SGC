using Core.Messages.Commands;

namespace Clientes.Application.Commands
{
    public class AtivarClienteCommand : CommandRequest
    {
        public string Id { get; private set; }
        public AtivarClienteCommand(string id)
        {
            Id = id;
        }
    }
}
