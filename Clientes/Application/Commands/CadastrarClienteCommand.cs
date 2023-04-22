﻿using Clientes.Domain.Model;
using Core.Messages.Commands;
using System.ComponentModel.DataAnnotations;

namespace Clientes.Application.Commands
{
    public class CadastrarClienteCommand : CommandRequest
    {
        public string Nome { get; set; } = null!;
        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; set; } = null!;
        public string Email { get; private set; } = string.Empty;
        public string Id { get; internal set; } = string.Empty;

        public void AdicionarEmail(string email)
        {
            Email = email;
        }
    }
}
