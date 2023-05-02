using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AplicacaoGerenciamentoLoja.CustomParameters.Venda
{
    public class StatusParametro
    {
        public StatusVendaEnum? Status { get; set; }
    }

    public enum StatusVendaEnum
    {
        [Display(Name = "confirmacao")]
        Confirmacao,
        [Display(Name = "cancelamento")]
        Cancelamento
    }
}
