namespace AplicacaoGerenciamentoLoja.CustomParameters
{
    public class CustomClientFilter
    {
        public TipoFiltro? Filtro { get; set; }
        public string? Valor { get; set; }

        public enum TipoFiltro
        {
            nome,
            cpf,
            email
        }
    }
}
