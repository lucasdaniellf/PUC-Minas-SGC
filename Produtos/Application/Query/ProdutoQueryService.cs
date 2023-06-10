using Core.Infrastructure;
using Produtos.Application.Query.DTO;
using Produtos.Domain.Model;
using Produtos.Domain.Repository;
using Produtos.Infrastructure;

namespace Produtos.Application.Query
{
    public class ProdutoQueryService
    {

        private readonly IUnitOfWork<Produto> _unitOfWork;
        private readonly IProdutoRepository _repository;

        public ProdutoQueryService(IUnitOfWork<Produto> unitOfWork, IProdutoRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<ProdutoQueryDto>> BuscarProdutos(CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var produtos = from produto in await _repository.BuscarProdutos(token) select MapQueryDto(produto);

                _unitOfWork.CloseConnection();
                return produtos;
            }
            catch (Exception) 
            {
                _unitOfWork.CloseConnection();
                throw;
            }
        }
        public async Task<IEnumerable<ProdutoQueryDto>> BuscarProdutoPorDescricao(string descricao, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var produtos = from produto in await _repository.BuscarProdutoPorDescricao(descricao, token) select MapQueryDto(produto);

                _unitOfWork.CloseConnection();
                return produtos;
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
        }

        //==================================================================================================================//

        public async Task<IEnumerable<ProdutoQueryDto>> BuscarProdutoPorId(string id, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var produtos = from produto in await _repository.BuscarProdutoPorId(id, token) select MapQueryDto(produto);

                _unitOfWork.CloseConnection();
                return produtos;
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
        }
        //==================================================================================================================//

        private ProdutoQueryDto MapQueryDto(Produto produto)
        {
            return new ProdutoQueryDto(produto.Id, produto.Descricao, produto.Preco, MapEstoqueDto(produto.Estoque), produto.Status);
        }

        private EstoqueDto MapEstoqueDto(Estoque estoque)
        {
            return new EstoqueDto(estoque.Quantidade, estoque.EstoqueMinimo);
        }
    }
}
