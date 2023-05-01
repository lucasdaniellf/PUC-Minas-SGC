using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clientes.Domain.Model
{
    public class Endereco
    {
        public string Rua { get; private set; } = null!;
        public string NumeroCasa { get; private set; } = null!;
        public string Complemento { get; private set; } = string.Empty;
        [RegularExpression(@"^\d{8}$")]
        public string CEP { get; private set; } = null!;
        public string Cidade { get; private set; } = null!;
        public string Bairro { get; private set; } = null!;
        public string Estado { get; private set; } = null!;

        public Endereco(string rua, string numeroCasa, string? complemento, string cep, string cidade, string bairro, string estado)
        {
            Rua = rua;
            NumeroCasa = numeroCasa;
            Complemento = complemento ?? string.Empty;
            CEP = cep;
            Cidade = cidade;
            Bairro = bairro;
            Estado = estado;
        }
    }
}
