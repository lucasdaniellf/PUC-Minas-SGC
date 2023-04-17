using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AplicacaoGerenciamentoLoja.CustomParameters.Venda
{
    public class DataParametro
    {
        [RegularExpression(@"^(\d{3}[1-9])-((0[1-9])|(1[012]))-((0[1-9])|([12]\d)|(3[01]))$")]
        public string DataInicio { get; set; } = null!;

        [RegularExpression(@"^(\d{3}[1-9])-((0[1-9])|(1[012]))-((0[1-9])|([12]\d)|(3[01]))$")]
        public string DataFim { get; set; } = null!;

        public DateTime FormatarDataInicio()
        {
            if (DateTime.TryParseExact(DataInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var formatedDate))
            {
                return formatedDate;
            }
            else
            {
                throw new ArgumentException("Data inválida", nameof(DataInicio));
            }
        }

        public DateTime FormatarDataFim()
        {
            if (DateTime.TryParseExact(DataFim, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var formatedDate))
            {
                return formatedDate;
            }
            else
            {
                throw new ArgumentException("Data inválida", nameof(DataFim));
            }
        }
    }
}
