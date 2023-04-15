﻿using Core.Entity;
using System.ComponentModel.DataAnnotations;
using static Clientes.Domain.Model.Status;

namespace Clientes.Domain.Model
{
    public class Cliente : IAggregateRoot
    {
        public string Id { get; private set; } = null!;
        public string Nome { get; private set; } = null!;
        [EmailAddress]
        public string Email { get; private set; } = null!;
        public CPF Cpf { get; private set; } = null!;
        public Status.ClienteStatus EstaAtivo { get; private set; } = Status.ClienteStatus.ATIVO;

        private Cliente(string Id, string Cpf, string Nome, string Email, long EstaAtivo) : this(Nome, Cpf, Email)
        {
            this.Id = Id;
            this.EstaAtivo = AplicarStatusEmCliente(EstaAtivo);
        }

        private Cliente(string nome, string cpf, string Email)
        {
            AtualizarNome(nome);
            AtualizarCpf(cpf);

            this.Email = Email;
        }

        public static Cliente CadastrarCliente(string nome, string cpf, string email)
        {
            Cliente cliente = new(nome, cpf, email)
            {
                Id = Guid.NewGuid().ToString()
            };

            return cliente;
        }

        //Em caso de necessidade de disparar algum evento relacionado à essa ação no futuro
        public void InativarCliente()
        {
            EstaAtivo = Status.ClienteStatus.INATIVO;
        }

        public void AtualizarStatusCliente(int status)
        {
            if(status != 0 && status != 1)
            {
                throw new ClienteException("Status inválido, deve ser 0 (inativo) ou 1 (ativo)");

            }
            EstaAtivo = Status.AplicarStatusEmCliente(status);
        }

        public void AtualizarNome(string nome)
        {
            if (string.IsNullOrEmpty(nome))
            {
                throw new ClienteException("Nome inválido, não deve ser vazio");
            }
            Nome = nome;
        }

        public void AtualizarCpf(string cpf)
        {
            CPF newCpf = new(cpf);
            Cpf = newCpf;
        }

        //public Cliente Apply(props...){...map...}
    }
}