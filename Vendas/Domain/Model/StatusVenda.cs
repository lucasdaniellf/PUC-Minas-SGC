namespace Vendas.Domain.Model
{
    public static class StatusVenda
    {
        public enum Status
        {
            PENDENTE = 0,
            PROCESSANDO = 1,
            AGUARDANDO_PAGAMENTO = 2,
            APROVADO = 3,
            REPROVADO = 4,
            CANCELADO = 99
        }

        public static Status AplicarStatus(int value)
        {
            switch (value)
            {
                case 0:
                    return Status.PENDENTE;
                case 1:
                    return Status.PROCESSANDO;
                case 2:
                    return Status.AGUARDANDO_PAGAMENTO;
                case 3:
                    return Status.APROVADO;
                case 4:
                    return Status.REPROVADO;
                default:
                    return Status.CANCELADO;
            }
        }
    }
}