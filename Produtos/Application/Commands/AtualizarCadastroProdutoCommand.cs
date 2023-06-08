using Core.Messages.Commands;
using static Produtos.Domain.Model.ProdutoStatus;

namespace Produtos.Application.Commands
{
    public class AtualizarCadastroProdutoCommand : CommandRequest
    {
        public string Id { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = null!;
        public decimal Preco { get; private set; }
        public int EstoqueMinimo { get; private set; }
        public ProdutoStatusEnum Status { get; set; }

        public AtualizarCadastroProdutoCommand(string id, string descricao, decimal preco, int estoqueMinimo, ProdutoStatusEnum status)
        {
            Id = id;
            Descricao = descricao;
            Preco = preco;
            EstoqueMinimo = estoqueMinimo;
            Status = status;
        }
    }
}
