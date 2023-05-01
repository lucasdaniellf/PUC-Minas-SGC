using Core.Entity;
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
        public ClienteStatus EstaAtivo { get; private set; } = ClienteStatus.ATIVO;
        public Endereco Endereco { get; private set; } = null!;

        internal Cliente(string Id, string Cpf, string Nome, string Email, long EstaAtivo, Endereco endereco) : this(Nome, Cpf, Email, endereco)
        {
            this.Id = Id;
            this.EstaAtivo = AplicarStatusEmCliente(EstaAtivo);
        }

        private Cliente(string nome, string cpf, string Email, Endereco endereco)
        {
            AtualizarNome(nome);
            AtualizarCpf(cpf);
            AtualizarEmail(Email);
            Endereco = endereco;
        }

        public static Cliente CadastrarCliente(string nome, string cpf, string email, Endereco endereco)
        {
            Cliente cliente = new(nome, cpf, email, endereco)
            {
                Id = Guid.NewGuid().ToString()
            };

            return cliente;
        }

        public void AtualizarDadosCliente(string nome, string cpf, string email, Endereco endereco)
        {
            if(this.EstaAtivo == ClienteStatus.INATIVO)
            {
                throw new ClienteException("Dados de usuário não podem ser atualizados pois seu status é INATIVO");
            }
            AtualizarNome(nome);
            AtualizarCpf(cpf);
            AtualizarEmail(email);
            Endereco = endereco;
        }

        public void AtualizarStatusCliente(ClienteStatus status)
        {
            if(!Enum.IsDefined(typeof(ClienteStatus), status))
            {
                throw new ClienteException("Status inválido, deve ser 0 (inativo) ou 1 (ativo)");
            }
            EstaAtivo = status;
        }

        private void AtualizarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ClienteException("Nome inválido, não deve ser vazio");
            }
            Nome = nome;
        }

        private void AtualizarCpf(string cpf)
        {
            CPF newCpf = new(cpf);
            Cpf = newCpf;
        }

        private void AtualizarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ClienteException("Email inválido, não deve ser vazio");
            }
            Email = email;
        }
    }
}
