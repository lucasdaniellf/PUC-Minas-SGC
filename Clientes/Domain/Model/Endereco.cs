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
        public string Rua { get; private set; } = string.Empty;
        public string NumeroCasa { get; private set; } = string.Empty;
        public string Complemento { get; private set; } = string.Empty;
        [RegularExpression(@"^\d{8}$")]
        public string CEP { get; private set; } = string.Empty;
        public string Cidade { get; private set; } = string.Empty;
        public string Bairro { get; private set; } = string.Empty;
        public string Estado { get; private set; } = string.Empty; 

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

        private Endereco()
        {

        }
    }
}
