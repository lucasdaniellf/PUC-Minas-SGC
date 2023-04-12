using Produtos.Domain.Model;

namespace Produtos.Domain.Repository
{
    public interface IProdutoRepository
    {
        public Task<IEnumerable<Produto>> BuscarProdutos(CancellationToken token);
        public Task<IEnumerable<Produto>> BuscarProdutoPorDescricao(string nome, CancellationToken token);
        public Task<IEnumerable<Produto>> BuscarProdutoPorId(string id, CancellationToken token);
        public Task<IEnumerable<Produto>> BuscarProdutoPorIdBloquearRegistro(string id, CancellationToken token);
        public Task<int> CadastrarProduto(Produto produto, CancellationToken token);
        public Task<int> AtualizarCadastroProduto(Produto produto, CancellationToken token);
        public Task<int> AdicionarProdutoACatalogo(string id, CancellationToken token);
        public Task<int> RetirarProdutoDeCatalogo(string id, CancellationToken token);
        public Task<int> AtualizarQuantidadeEstoqueProduto(Estoque estoque, CancellationToken token);
        public Task<int> AtualizarEstoqueMinimoProduto(Estoque estoque, CancellationToken token);

        public Task<int> GerarLogEstoque(LogEstoque log, CancellationToken token);

        //public Task<int> DiminuirQuantidadeEstoque(string ProdutoId, int Quantidade, CancellationToken token);

    }
}
