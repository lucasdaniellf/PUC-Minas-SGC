using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clientes.Application.Commands
{
    public class AtualizarCadastroClienteCommand : CommandRequest
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }

        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; private set; }

        public AtualizarCadastroClienteCommand(string id, string nome, string cpf)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
        }
    }
}
