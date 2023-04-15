using Core.Infrastructure;
using Dapper;
using Vendas.Domain.Model;
using Vendas.Domain.Repository;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Infrastructure
{
    public class VendaRepository : IVendaRepository
    {
        private readonly IDbContext<Venda> _context;

        public VendaRepository(IDbContext<Venda> context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Venda>> BuscarVendas(CancellationToken token)
        {
            var query = @"select 
                            v.Id, 
                            v.DataVenda, 
                            v.Desconto, 
                            v.FormaPagamento, 
                            v.Status, 
                            c.Id as ClienteId, 
                            c.EstaAtivo as ClienteStatus,
                            i.VendaId,
                            i.Quantidade,
                            i.ValorPago,
                            p.Id as ProdutoId,
                            p.Preco as PrecoProduto,
                            p.QuantidadeEstoque as QuantidadeEstoque,
                            p.EstaAtivo as ProdutoStatus
                        from Venda v
                        inner join Cliente c on v.ClienteId = c.Id
                        left join ItemVenda i on i.VendaId = v.Id
                        left join Produto p on p.Id = i.ProdutoId
            ";
            var vendaDictionary = new Dictionary<string, VendaTO>();
            var vendas = await _context.Connection.QueryAsync<VendaTO, ItemVendaTO, VendaTO>( new CommandDefinition(commandText: query,
                                                                                                transaction: _context.Transaction,
                                                                                                commandType: System.Data.CommandType.Text,
                                                                                                cancellationToken: token), (v, i) => VendaCustomMapping(vendaDictionary, v, i), 
                                                                                                splitOn: "VendaId");

            return from venda in vendas select VendaDataObject.MapearVendaDO(venda);
        }

        public async Task<IEnumerable<Venda>> BuscarVendaPorId(string id, CancellationToken token)
        {
            var query = @"select 
                            v.Id, 
                            v.DataVenda, 
                            v.Desconto, 
                            v.FormaPagamento, 
                            v.Status, 
                            c.Id as ClienteId, 
                            c.EstaAtivo as ClienteStatus,
                            i.VendaId,
                            i.Quantidade,
                            i.ValorPago,
                            p.Id as ProdutoId,
                            p.Preco as PrecoProduto,
                            p.QuantidadeEstoque as QuantidadeEstoque,
                            p.EstaAtivo as ProdutoStatus
                        from Venda v
                        inner join Cliente c on v.ClienteId = c.Id
                        left join ItemVenda i on i.VendaId = v.Id
                        left join Produto p on p.Id = i.ProdutoId
                        where v.id = @id";
            var vendaDictionary = new Dictionary<string, VendaTO>();
            var vendas = await _context.Connection.QueryAsync<VendaTO, ItemVendaTO, VendaTO>(new CommandDefinition(commandText: query,
                                                                                                parameters: new { id },
                                                                                                transaction: _context.Transaction,
                                                                                                commandType: System.Data.CommandType.Text,
                                                                                                cancellationToken: token), (v, i) =>
                                                                                                {
                                                                                                    return VendaCustomMapping(vendaDictionary, v, i);
                                                                                                }, splitOn:"VendaId");

            return from venda in vendas select VendaDataObject.MapearVendaDO(venda);
        }
        public async Task<IEnumerable<Venda>> BuscarVendasPorCliente(string clienteId, CancellationToken token)
        {
            var query = @"select 
                            v.Id, 
                            v.DataVenda, 
                            v.Desconto, 
                            v.FormaPagamento, 
                            v.Status, 
                            c.Id as ClienteId, 
                            c.EstaAtivo as ClienteStatus,
                            i.VendaId,
                            i.Quantidade,
                            i.ValorPago,
                            p.Id as ProdutoId,
                            p.Preco as PrecoProduto,
                            p.QuantidadeEstoque as QuantidadeEstoque,
                            p.EstaAtivo as ProdutoStatus
                        from Venda v
                        inner join Cliente c on v.ClienteId = c.Id
                        left join ItemVenda i on i.VendaId = v.Id
                        left join Produto p on p.Id = i.ProdutoId
                           where v.ClienteId = @ClienteId";
            var vendaDictionary = new Dictionary<string, VendaTO>();
            var vendas = await _context.Connection.QueryAsync<VendaTO, ItemVendaTO, VendaTO>(new CommandDefinition(commandText: query,
                                                                                                parameters: new { ClienteId = clienteId },
                                                                                                transaction: _context.Transaction,
                                                                                                commandType: System.Data.CommandType.Text,
                                                                                                cancellationToken: token), (v, i) =>
                                                                                                {
                                                                                                    return VendaCustomMapping(vendaDictionary, v, i);
                                                                                                }, splitOn: "VendaId");
            return from venda in vendas select VendaDataObject.MapearVendaDO(venda);

        }

        public async Task<IEnumerable<Venda>> BuscarVendasPorData(DateTime dataInicio, DateTime dataFim, CancellationToken token)
        {
            var query = @"select 
                            v.Id, 
                            v.DataVenda, 
                            v.Desconto, 
                            v.FormaPagamento, 
                            v.Status, 
                            c.Id as ClienteId, 
                            c.EstaAtivo as ClienteStatus,
                            i.VendaId,
                            i.Quantidade,
                            i.ValorPago,
                            p.Id as ProdutoId,
                            p.Preco as PrecoProduto,
                            p.QuantidadeEstoque as QuantidadeEstoque,
                            p.EstaAtivo as ProdutoStatus
                        from Venda v
                        inner join Cliente c on v.ClienteId = c.Id
                        left join ItemVenda i on i.VendaId = v.Id
                        left join Produto p on p.Id = i.ProdutoId
                        where v.DataVenda between @DataInicio and @DataFim";
            var vendaDictionary = new Dictionary<string, VendaTO>();
            var vendas = await _context.Connection.QueryAsync<VendaTO, ItemVendaTO, VendaTO>(new CommandDefinition(commandText: query,
                                                                                                parameters: new { DataInicio = dataInicio.Date.ToString("yyyy-MM-dd"), DataFim = dataFim.Date.ToString("yyyy-MM-dd") },
                                                                                                transaction: _context.Transaction,
                                                                                                commandType: System.Data.CommandType.Text,
                                                                                                cancellationToken: token), (v, i) =>
                                                                                                {
                                                                                                    return VendaCustomMapping(vendaDictionary, v, i);
                                                                                                }, splitOn: "VendaId");

            return from venda in vendas select VendaDataObject.MapearVendaDO(venda);
        }

        public async Task<IEnumerable<Venda>> BuscarVendaPorStatusVenda(Status status, CancellationToken token)
        {
            var query = @"select 
                            v.Id, 
                            v.DataVenda, 
                            v.Desconto, 
                            v.FormaPagamento, 
                            v.Status, 
                            c.Id as ClienteId, 
                            c.EstaAtivo as ClienteStatus,
                            i.VendaId,
                            i.Quantidade,
                            i.ValorPago,
                            p.Id as ProdutoId,
                            p.Preco as PrecoProduto,
                            p.QuantidadeEstoque as QuantidadeEstoque,
                            p.EstaAtivo as ProdutoStatus
                        from Venda v
                        inner join Cliente c on v.ClienteId = c.Id
                        left join ItemVenda i on i.VendaId = v.Id
                        left join Produto p on p.Id = i.ProdutoId
                        where v.Status = @Status";
            var vendaDictionary = new Dictionary<string, VendaTO>();
            var vendas = await _context.Connection.QueryAsync<VendaTO, ItemVendaTO, VendaTO>(new CommandDefinition(commandText: query,
                                                                                                parameters: new { Status = status },
                                                                                                transaction: _context.Transaction,
                                                                                                commandType: System.Data.CommandType.Text,
                                                                                                cancellationToken: token), (v, i) =>
                                                                                                {
                                                                                                    return VendaCustomMapping(vendaDictionary, v, i);
                                                                                                }, splitOn: "VendaId");

            return from venda in vendas select VendaDataObject.MapearVendaDO(venda);
        }

        public async Task<int> CadastrarVenda(Venda venda, CancellationToken token)
        {
            var query = @"insert into venda(id, clienteId, dataVenda, desconto, status, formaPagamento) values
                            (@id, @clienteId, @dataVenda, 0, 0, 0)";

            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                    parameters: new {id = venda.Id, clienteId = venda.Cliente.Id, dataVenda = venda.DataVenda},
                                                                                    transaction: _context.Transaction,
                                                                                    commandType: System.Data.CommandType.Text,
                                                                                    cancellationToken: token));
        }

        public async Task<int> AtualizarVenda(Venda venda, CancellationToken token)
        {
            var query = @"update venda set desconto = @desconto, status = @status, formaPagamento = @formaDePagamento, dataVenda = @dataVenda 
                            where id = @id";

            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                    parameters: new { desconto = venda.Desconto,
                                                                                                        status = venda.Status,
                                                                                                        formaDePagamento = venda.FormaDePagamento,
                                                                                                        dataVenda = venda.DataVenda,
                                                                                                        id = venda.Id },
                                                                                    transaction: _context.Transaction,
                                                                                    commandType: System.Data.CommandType.Text,
                                                                                    cancellationToken: token));

        }


        public async Task<int> AdicionarProdutoEmVenda(ItemVenda item, CancellationToken token)
        {
            var query = @"insert into ItemVenda(VendaId, ProdutoId, ValorPago, Quantidade) values
                            (@VendaId, @ProdutoId, @ValorPago, @Quantidade)";

            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                    parameters: new { VendaId = item.Venda.Id, ProdutoId = item.Produto.Id, item.ValorPago, item.Quantidade },
                                                                                    transaction: _context.Transaction,
                                                                                    commandType: System.Data.CommandType.Text,
                                                                                    cancellationToken: token));
        }

        public async Task<int> RemoverProdutoEmVenda(ItemVenda item, CancellationToken token)
        {
            var query = @"delete from ItemVenda where ProdutoId = @ProdutoId and VendaId = @VendaId";

            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                    parameters: new { VendaId = item.Venda.Id, ProdutoId = item.Produto.Id },
                                                                                    transaction: _context.Transaction,
                                                                                    commandType: System.Data.CommandType.Text,
                                                                                    cancellationToken: token));
        }


        public async Task<int> AtualizarProdutoEmVenda(ItemVenda item, CancellationToken token)
        {
            var query = @"update ItemVenda set ValorPago = @ValorPago, Quantidade = @Quantidade where VendaId = @VendaId and ProdutoId = @ProdutoId";

            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                    parameters: new { VendaId = item.Venda.Id, ProdutoId = item.Produto.Id, item.ValorPago, item.Quantidade },
                                                                                    transaction: _context.Transaction,
                                                                                    commandType: System.Data.CommandType.Text,
                                                                                    cancellationToken: token));
        }

        private VendaTO VendaCustomMapping(Dictionary<string, VendaTO> vendas, VendaTO v, ItemVendaTO i)
        {
            if (!vendas.TryGetValue(v.Id, out var venda))
            {
                venda = v;
                vendas.Add(v.Id, venda);
            }
            if(i != null)
            {
                venda.itens.Add(i);
            }
            return venda;
        }
        //=====================Produto=====================//

        public async Task<IEnumerable<ProdutoVenda>> BuscarProdutos(CancellationToken token)
        {
            string sql = @"select p.* from Produto p";
            return await _context.Connection.QueryAsync<ProdutoVenda>(new CommandDefinition(commandText: sql,
                                                                               transaction: _context.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));
        }

        public async Task<IEnumerable<ProdutoVenda>> BuscarProdutoPorId(string id, CancellationToken token)
        {
            string sql = @"select p.* from Produto p
                            where p.Id = @Id";
            return await _context.Connection.QueryAsync<ProdutoVenda>(new CommandDefinition(commandText: sql,
                                                                               parameters: new { Id = id },
                                                                               transaction: _context.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));
        }
        public async Task<int> CadastrarProduto(ProdutoVenda produto, CancellationToken token)
        {
            string sql = @"insert into Produto(Id, Preco, EstaAtivo, QuantidadeEstoque) values (@Id, @Preco, @EstaAtivo, @QuantidadeEstoque);";
            var row = await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new
                                                                               {
                                                                                   produto.Id,
                                                                                   produto.Preco,
                                                                                   produto.EstaAtivo,
                                                                                   produto.QuantidadeEstoque
                                                                                },
                                                                               transaction: _context.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));

            return row;
        }

        public async Task<int> AtualizarCadastroProduto(ProdutoVenda produto, CancellationToken token)
        {
            string sql = @"update Produto set Preco = @Preco, QuantidadeEstoque = @QuantidadeEstoque, EstaAtivo = @EstaAtivo where Id = @Id";
            var row = await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: sql,
                                                                               parameters: new { produto.Id, produto.Preco, produto.QuantidadeEstoque, produto.EstaAtivo },
                                                                               transaction: _context.Transaction,
                                                                               commandType: System.Data.CommandType.Text,
                                                                               cancellationToken: token));

            return row;
        }
        //=================Cliente====================//

        public async Task<IEnumerable<ClienteVenda>> BuscarClientes(CancellationToken token)
        {
            var query = @"select * from Cliente";
            return await _context.Connection.QueryAsync<ClienteVenda>(new CommandDefinition(commandText: query,
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
        }


        public async Task<IEnumerable<ClienteVenda>> BuscarClientePorId(string id, CancellationToken token)
        {
            var query = @"select * from Cliente where Id = @Id";
            return await _context.Connection.QueryAsync<ClienteVenda>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Id = id },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
        }

        public async Task<int> AtualizarCliente(ClienteVenda cliente, CancellationToken token)
        {
            var query = @"update Cliente set EstaAtivo = @EstaAtivo where Id = @Id";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                            parameters: new { Id = cliente.Id, EstaAtivo = cliente.Status },
                                                                                            transaction: _context.Transaction,
                                                                                            commandType: System.Data.CommandType.Text,
                                                                                            cancellationToken: token));
        }


        public async Task<int> CadastrarCliente(ClienteVenda cliente, CancellationToken token)
        {
            var query = @"insert into Cliente(Id, EstaAtivo) values (@Id, @EstaAtivo)";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                             parameters: new
                                                                                             {
                                                                                                 Id = cliente.Id.ToString(),
                                                                                                 EstaAtivo = cliente.Status

                                                                                             },
                                                                                             transaction: _context.Transaction,
                                                                                             commandType: System.Data.CommandType.Text,
                                                                                             cancellationToken: token));
        }
    }
}
