namespace Vendas.Domain
{
    public class VendaDomainSettings
    {
        public string FilaReservarProduto { get; set; } = null!;
        public string FilaGerarFatura { get; set; } = null!;
        public string FilaFaturamentoCallback { get; set; } = null!;
        public string FilaReporProduto { get; set; } = null!;
    }
}
