using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clientes.Domain.Model
{
    public class Endereco
    {
        public string Rua { get; set; } = null!;
        public string NumeroCasa { get; set; } = null!;
        public string Complemento { get; set; } = string.Empty;
        public string CEP { get; set; } = null!;
        public string Cidade { get; set; } = null!;
        public string Bairro { get; set; } = null!;
        public string Pais { get; set; } = null!;

        public Endereco(string rua, string numeroCasa, string complemento, string cEP, string cidade, string bairro, string pais)
        {
            Rua = rua;
            NumeroCasa = numeroCasa;
            Complemento = complemento;
            CEP = cEP;
            Cidade = cidade;
            Bairro = bairro;
            Pais = pais;
        }
    }
}
