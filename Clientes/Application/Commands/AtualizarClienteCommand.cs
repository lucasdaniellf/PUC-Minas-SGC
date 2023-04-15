using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clientes.Application.Commands
{
    public class AtualizarClienteCommand : CommandRequest
    {
        public string Id { get; private set; } = string.Empty;
        public string Nome { get; private set; }

        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; private set; }

        public int Status { get; private set; }

        public AtualizarClienteCommand(string nome, string cpf, int status)
        {
            Nome = nome;
            Cpf = cpf;
            Status = status;
        }

        public void AdicionarId(string id)
        {
            Id = id;
        }
    }
}
