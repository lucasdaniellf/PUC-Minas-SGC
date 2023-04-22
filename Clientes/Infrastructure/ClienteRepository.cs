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
            var query = @"update Cliente set Nome = @Nome, Cpf = @Cpf, EstaAtivo = @EstaAtivo  where Id = @Id";
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
            var query = @"select c.*,
                             Rua, 
                             NumeroCasa,
                             Complemento,
                             CEP, 
                             Bairro,
                             Cidade,
                             Estado
                        from Cliente c 
                             Join Endereco e on e.ClienteId = c.Id
                        where Cpf = @Cpf";
            return await _context.Connection.QueryAsync<Cliente>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Cpf = cpf },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
        }
        public async Task<IEnumerable<Cliente>> BuscarClientePorEmail(string email, CancellationToken token)
        {
            var query = @"select *
                             Rua, 
                             NumeroCasa,
                             Complemento,
                             CEP, 
                             Bairro,
                             Cidade,
                             Estado
                        from Cliente c 
                             Join Endereco e on e.ClienteId = c.Id
                            from Cliente where Email = @Email";
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
            var query = @"select c.* from Cliente c";
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

        //==========Endereco=========================================================================================//
        public async Task<int> CadastrarEndereco(Cliente cliente, CancellationToken token)
        {
            var query = @"insert into Endereco(ClienteId, Rua, NumeroCasa, Complemento, CEP, Bairro, Cidade, Estado) 
                            values (@ClienteId, @Rua, @NumeroCasa, @Complemento, @CEP, @Bairro, @Cidade, @Estado)";

            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                parameters: new
                                                                                {
                                                                                    ClienteId = cliente.Id,
                                                                                    Rua = cliente.Endereco.Rua,
                                                                                    NumeroCasa = cliente.Endereco.NumeroCasa,
                                                                                    Complemento = cliente.Endereco.Complemento,
                                                                                    CEP = cliente.Endereco.CEP,
                                                                                    Bairro = cliente.Endereco.Bairro,
                                                                                    Cidade = cliente.Endereco.Cidade,
                                                                                    Estado = cliente.Endereco.Estado
                                                                                },
                                                                                transaction: _context.Transaction,
                                                                                commandType: System.Data.CommandType.Text,
                                                                                cancellationToken: token));
        }

        public async Task<int> AtualizarEndereco(Cliente cliente, CancellationToken token)
        {
            var query = @"uptade Endereco set Rua = @Rua, 
                                                NumeroCasa = @NumeroCasa, 
                                                Complemento = @Complemento, 
                                                CEP = @CEP,
                                                Bairro = @Bairro,
                                                Cidade = @Cidade, 
                                                Estado = @Estado
                        where ClienteId = @ClienteId";

            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                parameters: new
                                                                                {
                                                                                    ClienteId = cliente.Id,
                                                                                    Rua = cliente.Endereco.Rua,
                                                                                    NumeroCasa = cliente.Endereco.NumeroCasa,
                                                                                    Complemento = cliente.Endereco.Complemento,
                                                                                    CEP = cliente.Endereco.CEP,
                                                                                    Bairro = cliente.Endereco.Bairro,
                                                                                    Cidade = cliente.Endereco.Cidade,
                                                                                    Estado = cliente.Endereco.Estado
                                                                                },
                                                                                transaction: _context.Transaction,
                                                                                commandType: System.Data.CommandType.Text,
                                                                                cancellationToken: token));
        }
    }
}
