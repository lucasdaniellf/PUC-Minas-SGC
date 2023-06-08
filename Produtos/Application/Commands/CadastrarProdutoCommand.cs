using Core.Messages.Commands;
namespace Produtos.Application.Commands
{
    public class CadastrarProdutoCommand : CommandRequest
    {
        public string Id { get; internal set; } = string.Empty;
        public string Descricao { get; private set; } = null!;
        public decimal Preco { get; private set; }

        public CadastrarProdutoCommand(string descricao, decimal preco)
        {
            Descricao = descricao;
            Preco = preco;
        }
    }
}
