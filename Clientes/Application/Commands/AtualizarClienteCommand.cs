using Clientes.Domain.Model;
using Core.Messages.Commands;
using System.ComponentModel.DataAnnotations;

namespace Clientes.Application.Commands
{
    public class AtualizarClienteCommand : CommandRequest
    {
        public string Id { get; set; } = null!;
        public string Nome { get; set; } = null!;

        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; set; } = null!;
        public Endereco Endereco { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;

        public AtualizarClienteCommand(string id, string nome, string cpf, Endereco endereco, string email)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Endereco = endereco;
            Email = email;
        }
    }
}
