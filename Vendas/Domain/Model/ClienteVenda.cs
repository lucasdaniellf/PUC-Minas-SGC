using Core.Entity;

namespace Vendas.Domain.Model
{
    public class ClienteVenda : IEntity
    {
        internal string Id { get; private set; } = null!;
        internal ClienteStatus Status { get; private set; }
        internal ClienteVenda(string id, long EstaAtivo)
        {
            Id = id;
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
