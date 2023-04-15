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

        public async Task<int> AtualizarCliente(Cliente cliente, CancellationToken token)
        {
            var query = @"update Cliente set Nome = @Nome, Cpf = @Cpf, @EstaAtivo = @EstaAtivo  where Id = @Id";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                             parameters: new
                                                                                             {
                                                                                                 cliente.Nome,
                                                                                                 Cpf = cliente.Cpf.Numero,
                                                                                                 cliente.EstaAtivo,
                                                                                                 cliente.Id
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
        public async Task<IEnumerable<Cliente>> BuscarClientePorEmail(string email, CancellationToken token)
        {
            var query = @"select * from Cliente where Email = @Email";
            return await _context.Connection.QueryAsync<Cliente>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Email = email },
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
            var query = @"insert into Cliente(Id, Nome, Cpf, Email, EstaAtivo) values (@Id, @Nome, @Cpf, @Email, 1)";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                             parameters: new
                                                                                             {
                                                                                                 Id = cliente.Id.ToString(),
                                                                                                 Cpf = cliente.Cpf.Numero,
                                                                                                 cliente.Email,
                                                                                                 cliente.Nome
                                                                                             },
                                                                                             transaction: _context.Transaction,
                                                                                             commandType: System.Data.CommandType.Text,
                                                                                             cancellationToken: token));
        }
    }
}
