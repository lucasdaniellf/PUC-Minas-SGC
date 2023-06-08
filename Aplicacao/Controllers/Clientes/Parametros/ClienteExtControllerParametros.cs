using Clientes.Domain.Model;

namespace AplicacaoGerenciamentoLoja.Controllers.Clientes.Parametros.ExternalController
{
    public record CadastrarClienteExternalRequest
    {
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public Endereco Endereco { get; set; } = null!;
    }

    public record AtualizarClienteExternalRequest
    {
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public Endereco Endereco { get; set; } = null!;
    }
}
