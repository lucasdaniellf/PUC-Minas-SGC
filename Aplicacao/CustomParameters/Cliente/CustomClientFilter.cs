namespace AplicacaoGerenciamentoLoja.CustomParameters.Cliente
{
    public class CustomClientFilter
    {
        public TipoFiltro Filtro { get; set; }
        public string Valor { get; set; } = null!;

        public enum TipoFiltro
        {
            cpf,
            email
        }
    }
}
