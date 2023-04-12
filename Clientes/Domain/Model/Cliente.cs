using Core.Entity;
using static Clientes.Domain.Model.Status;

namespace Clientes.Domain.Model
{
    public class Cliente : IAggregateRoot
    {
        public string Id { get; private set; } = null!;
        public string Nome { get; private set; } = null!;
        public CPF Cpf { get; private set; } = null!;
        public Status.ClienteStatus EstaAtivo { get; private set; } = Status.ClienteStatus.ATIVO;

        private Cliente(string Id, string Cpf, string Nome, long EstaAtivo) : this(Nome, Cpf)
        {
            this.Id = Id;
            this.EstaAtivo = AplicarStatusEmCliente(EstaAtivo);
        }

        private Cliente(string nome, string cpf)
        {
            AtualizarNome(nome);
            AtualizarCpf(cpf);
        }

        public static Cliente CadastrarCliente(string nome, string cpf)
        {
            Cliente cliente = new(nome, cpf)
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

        public void AtivarCliente()
        {
            EstaAtivo = Status.ClienteStatus.ATIVO;
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
