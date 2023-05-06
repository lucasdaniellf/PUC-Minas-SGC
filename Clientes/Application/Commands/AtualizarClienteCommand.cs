using Clientes.Domain.Model;
using Core.Messages.Commands;
using System.ComponentModel.DataAnnotations;

namespace Clientes.Application.Commands
{
    public class AtualizarClienteCommand : CommandRequest
    {
        public string Id { get; private set; } = string.Empty;
        public string Nome { get; set; } = null!;

        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; set; } = null!;
        public Endereco Endereco { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public void AdicionarId(string id)
        {
            Id = id;
        }
    }
}
