using Core.Infrastructure;
using Dapper;
using Produtos.Domain.Model;
using Produtos.Domain.Repository;

namespace Produtos.Infrastructure
{
    public class ProdutoRepository : IProdutoRepository
    {

        private readonly IDbContext<Produto> _dbContext;
        public ProdutoRepository(IDbContext<Produto> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Produto>> BuscarProdutos(CancellationToken token)
        {
            string sql = @"select p.*, e.Id as EstoqueId, e.Quantidade, e.EstoqueMinimo, e.UltimaAlteracao
                            from Produto p
                            inner join Estoque e on p.Id = e.ProdutoId";
            var produtos = await _dbContext.Connection.QueryAsync<ProdutoTO>(new CommandDefinition(commandText: sql,
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));

            return from p in produtos select ProdutoRepositoryMapping.MapearProdutoEstoque(p);
        }

        public async Task<IEnumerable<Produto>> BuscarProdutoPorId(string id, CancellationToken token)
        {
            string sql = @"select p.*, e.Id as EstoqueId, e.Quantidade, e.EstoqueMinimo,  e.UltimaAlteracao from Produto p
                            inner join Estoque e on p.Id = e.ProdutoId
                            where p.Id = @Id";
            var produtos = await _dbContext.Connection.QueryAsync<ProdutoTO>(new CommandDefinition(commandText: sql,
                                                                               parameters: new { Id = id },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));
            return from p in produtos select ProdutoRepositoryMapping.MapearProdutoEstoque(p);
        }

        public async Task<IEnumerable<Produto>> BuscarProdutoPorDescricao(string descricao, CancellationToken token)
        {
            string sql = @"select p.*, e.Id as EstoqueId, e.Quantidade, e.EstoqueMinimo, e.UltimaAlteracao from Produto p
                            inner join Estoque e on p.Id = e.ProdutoId
                            where UPPER(p.Descricao) like @Descricao";
            var produtos = await _dbContext.Connection.QueryAsync<ProdutoTO>(new CommandDefinition(commandText: sql,
                                                                               parameters: new { Descricao = string.Concat("%", descricao.ToUpper(), "%") },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));

            return from p in produtos select ProdutoRepositoryMapping.MapearProdutoEstoque(p);
        }

        public async Task<int> CadastrarProduto(Produto produto, CancellationToken token)
        {
            string sql = @"insert into Produto(Id, Descricao, Preco, EstaAtivo) values (@Id, @Descricao, @Preco, 1);
                           insert into Estoque(Id, Quantidade, EstoqueMinimo, UltimaAlteracao, ProdutoId) values (@EstoqueId, @Quantidade, @EstoqueMinimo, @UltimaAlteracao, @ProdutoId);
                          ";
            var row = await _dbContext.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new
                                                                               {
                                                                                   produto.Id,
                                                                                   produto.Descricao,
                                                                                   produto.Preco,

                                                                                   EstoqueId = produto.Estoque.Id,
                                                                                   produto.Estoque.Quantidade,
                                                                                   EstoqueMinimo = produto.Estoque.EstoqueMinimo,
                                                                                   UltimaAlteracao = produto.Estoque.UltimaAlteracao.ToString(),
                                                                                   ProdutoId = produto.Id,
                                                                               },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));


            return row;
        }

        public async Task<int> AtualizarCadastroProduto(Produto produto, CancellationToken token)
        {
            string sql = @"update Produto set Descricao = @Descricao, Preco = @Preco, EstaAtivo = @EstaAtivo where Id = @Id";
            var row = await _dbContext.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new { produto.Id, produto.Descricao, produto.Preco, produto.EstaAtivo },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));

            return row;
        }

        public Task<int> AdicionarProdutoACatalogo(string id, CancellationToken token)
        {
            string sql = @"update Produto set EstaAtivo = 1 where Id = @Id";
            var row = _dbContext.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new { Id = id },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));
            return row;
        }
        public Task<int> RetirarProdutoDeCatalogo(string id, CancellationToken token)
        {
            string sql = @"update Produto set EstaAtivo = 0 where Id = @Id";
            var row = _dbContext.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new { Id = id },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));
            return row;
        }

        public async Task<int> AtualizarEstoqueMinimoProduto(Estoque estoque, CancellationToken token)
        {
            string sql = @"update Estoque set EstoqueMinimo = @EstoqueMinimo where Id = @Id";
            var row = await _dbContext.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new { estoque.EstoqueMinimo, estoque.Id },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));
            return row;
        }


        public async Task<int> AtualizarQuantidadeEstoqueProduto(Estoque estoque, CancellationToken token)
        {
            string sql = @"update Estoque set Quantidade = @Quantidade where Id = @Id";
            var row = await _dbContext.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new { estoque.Quantidade, estoque.Id },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));
            return row;
        }

        public async Task<int> GerarLogEstoque(LogEstoque log, CancellationToken token)
        {
            string sql = @"insert into LogEstoque(EstoqueId, HorarioAlteracao, Quantidade) values (@EstoqueId, @HorarioAtualizacao, @Quantidade)";
            var row = await _dbContext.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                       parameters: new
                                                       {
                                                           log.EstoqueId,
                                                           log.Quantidade,
                                                           HorarioAtualizacao = log.HorarioAtualizacao.ToString()
                                                       },
                                                       transaction: _dbContext.Transaction,
                                                       commandType: System.Data.CommandType.Text,
                                                       cancellationToken: token));

            return row;

        }

        public async Task<IEnumerable<Produto>> BuscarProdutoPorIdBloquearRegistro(string id, CancellationToken token)
        {
            string sql = @"update Estoque set UltimaAlteracao = @UltimaAlteracao where ProdutoId = @ProdutoId";
            await _dbContext.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new { UltimaAlteracao = DateTime.Now.ToString(), ProdutoId = id },
                                                                               transaction: _dbContext.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));

            return await BuscarProdutoPorId(id, token);
        }
    }
}
