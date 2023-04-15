namespace Clientes.Domain.Model
{
    public class Status
    {
        public enum ClienteStatus
        {
            INATIVO = 0,
            ATIVO = 1
        }

        public static ClienteStatus AplicarStatusEmCliente(long value)
        {
            return value == 0 ? ClienteStatus.INATIVO : ClienteStatus.ATIVO;
        }
    }
}
