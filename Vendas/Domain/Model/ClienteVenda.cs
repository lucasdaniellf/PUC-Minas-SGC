using Core.Entity;

namespace Vendas.Domain.Model
{
    public class ClienteVenda : IEntity
    {
        internal string Id { get; private set; } = null!;
        internal ClienteStatus Status { get; private set; }
        internal string Email { get; private set; } = null!;

        internal ClienteVenda(string id, string email, long EstaAtivo)
        {
            Id = id;
            Email = email;
            AplicarStatusEmCliente(EstaAtivo);
        }

        internal enum ClienteStatus
        {
            INATIVO = 0,
            ATIVO = 1
        }

        internal void AplicarStatusEmCliente(long value)
        {
            this.Status = value == 0 ? ClienteStatus.INATIVO : ClienteStatus.ATIVO;
        }
    }
}
