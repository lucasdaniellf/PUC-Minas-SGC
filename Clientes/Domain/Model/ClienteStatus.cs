namespace Clientes.Domain.Model
{
    public class ClienteStatus
    {
        public enum ClienteStatusEnum
        {
            INATIVO = 0,
            ATIVO = 1
        }

        public static ClienteStatusEnum AplicarStatusEmCliente(long value)
        {
            return value == 0 ? ClienteStatusEnum.INATIVO : ClienteStatusEnum.ATIVO;
        }
    }
}
