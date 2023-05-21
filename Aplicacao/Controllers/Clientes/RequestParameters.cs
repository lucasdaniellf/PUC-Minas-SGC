using Clientes.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace AplicacaoGerenciamentoLoja.Controllers.Clientes
{
    public record CriarClienteRequest
    {
        public string Nome { get; set; } = null!;
        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; set; } = null!;
        public Endereco Endereco { get; set; } = null!;
    }

    public record AtualizarClienteRequest
    {
        public string Nome { get; set; } = null!;
        [RegularExpression(@"^\d{11}$")]
        public string Cpf { get; set; } = null!;
        public Endereco Endereco { get; set; } = null!;
    }
}
