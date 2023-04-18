using Clientes.Domain.Model;
using Core.Messages.Commands;
using System.ComponentModel.DataAnnotations;

namespace Clientes.Application.Commands
{
    public class CadastrarClienteCommand : CommandRequest
    {
        public string Nome { get; set; } = null!;
        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Id { get; internal set; } = string.Empty;
    }
}
