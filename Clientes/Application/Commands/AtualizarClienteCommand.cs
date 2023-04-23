using Clientes.Domain.Model;
using Core.Messages.Commands;
using System.ComponentModel.DataAnnotations;
using static Clientes.Domain.Model.Status;

namespace Clientes.Application.Commands
{
    public class AtualizarClienteCommand : CommandRequest
    {
        public string Id { get; private set; } = string.Empty;
        public string Nome { get; set; } = null!;

        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; set; } = null!;

        [RegularExpression(@"^\d{8}$")]
        public Endereco Endereco { get; set; } = null!;
        //-----------------------------------------------------//
        //public ClienteStatus EstaAtivo { get; set; }

        public void AdicionarId(string id)
        {
            Id = id;
        }
    }
}
