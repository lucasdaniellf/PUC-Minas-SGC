using static Vendas.Domain.Model.FormaPagamentoEnum;

namespace AplicacaoGerenciamentoLoja.CustomParameters.Venda
{
    public class FormaPagamentoParametro
    {
        public FormaPagamento? Modalidade { get; set; }
    }
}
