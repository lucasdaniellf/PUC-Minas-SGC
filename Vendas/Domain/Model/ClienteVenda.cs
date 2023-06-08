using Core.Entity;
using Newtonsoft.Json;

namespace Vendas.Domain.Model
{
    public class ClienteVenda : IEntity
    {
        public string Id { get; private set; } = null!;
        public ClienteStatus Status { get; private set; }
        public string Email { get; private set; } = null!;

        internal ClienteVenda(string id, string email, long status)
        {
            Id = id;
            Email = email;
            AplicarStatusEmCliente(status);
        }

        public enum ClienteStatus
        {
            INATIVO = 0,
            ATIVO = 1
        }

        public void AplicarStatusEmCliente(long value)
        {
            this.Status = value == 0 ? ClienteStatus.INATIVO : ClienteStatus.ATIVO;
        }

        
    }
}
