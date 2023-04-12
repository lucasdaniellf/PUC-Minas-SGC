using Core.Messages.Commands;

namespace Clientes.Application.Commands
{
    public class InativarClienteCommand : CommandRequest
    {
        public string Id { get; private set; }
        public InativarClienteCommand(string id)
        {
            Id = id;
        }
    }
}