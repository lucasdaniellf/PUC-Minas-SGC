using Clientes.Domain.Model;

namespace AplicacaoGerenciamentoLoja.Controllers.Clientes.Parametros.InternalController
{
    public record CadastrarClienteInternalRequest
    {
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public Endereco Endereco { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public record AtualizarClienteInternalRequest
    {
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public Endereco Endereco { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
