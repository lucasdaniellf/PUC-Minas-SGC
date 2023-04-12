using Clientes.Domain.Model;
using Clientes.Domain.Repository;
using Core.Infrastructure;
using Dapper;

namespace Clientes.Infrastructure
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbContext<Cliente> _context;
        public ClienteRepository(IDbContext<Cliente> context)
        {
            _context = context;
        }

        public async Task<int> AtivarCliente(string id, CancellationToken token)
        {
            var query = @"update Cliente set EstaAtivo = 1 where Id = @Id";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                            parameters: new { Id = id },
                                                                                            transaction: _context.Transaction,
                                                                                            commandType: System.Data.CommandType.Text,
                                                                                            cancellationToken: token));
        }

        public async Task<int> AtualizarCadastroCliente(Cliente cliente, CancellationToken token)
        {
            var query = @"update Cliente set Nome = @Nome where Cpf = @Cpf";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                             parameters: new
                                                                                             {
                                                                                                 Cpf = cliente.Cpf.Numero,
                                                                                                 cliente.Nome
                                                                                             },
                                                                                             transaction: _context.Transaction,
                                                                                             commandType: System.Data.CommandType.Text,
                                                                                             cancellationToken: token));
        }

        public async Task<IEnumerable<Cliente>> BuscarClientePorCPF(string cpf, CancellationToken token)
        {
            var query = @"select * from Cliente where Cpf = @Cpf";
            return await _context.Connection.QueryAsync<Cliente>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Cpf = cpf },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
        }

        public async Task<IEnumerable<Cliente>> BuscarClientePorId(string id, CancellationToken token)
        {
            var query = @"select * from Cliente where Id = @Id";
            return await _context.Connection.QueryAsync<Cliente>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Id = id },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
        }

        public async Task<IEnumerable<Cliente>> BuscarClientePorNome(string nome, CancellationToken token)
        {
            var query = @"select * from Cliente where UPPER(Nome) like @Nome";
            return await _context.Connection.QueryAsync<Cliente>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Nome = string.Concat("%", nome.ToUpper(), "%") },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
        }

        public async Task<IEnumerable<Cliente>> BuscarClientes(CancellationToken token)
        {
            var query = @"select * from Cliente";
            return await _context.Connection.QueryAsync<Cliente>(new CommandDefinition(commandText: query,
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
        }

        public async Task<int> CadastrarCliente(Cliente cliente, CancellationToken token)
        {
            var query = @"insert into Cliente(Id, Nome, Cpf, EstaAtivo) values (@Id, @Nome, @Cpf, 1)";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                             parameters: new
                                                                                             {
                                                                                                 Id = cliente.Id.ToString(),
                                                                                                 Cpf = cliente.Cpf.Numero,
                                                                                                 cliente.Nome
                                                                                             },
                                                                                             transaction: _context.Transaction,
                                                                                             commandType: System.Data.CommandType.Text,
                                                                                             cancellationToken: token));
        }

        public async Task<int> InativarCliente(string id, CancellationToken token)
        {
            var query = @"update Cliente set EstaAtivo = 0 where Id = @Id";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                            parameters: new { Id = id },
                                                                                            transaction: _context.Transaction,
                                                                                            commandType: System.Data.CommandType.Text,
                                                                                            cancellationToken: token));
        }
    }
}
