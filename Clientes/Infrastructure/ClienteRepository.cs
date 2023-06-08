using Clientes.Domain.Model;
using Clientes.Domain.Repository;
using Core.Infrastructure;
using Dapper;
using System.Collections.Generic;
using static Clientes.Infrastructure.ClienteDataObject;

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
            var query = @"update Cliente set Nome = @Nome, Cpf = @Cpf, Status = @Status, Email = @Email  where Id = @Id";
            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                             parameters: new
                                                                                             {
                                                                                                 cliente.Nome,
                                                                                                 Cpf = cliente.Cpf.Numero,
                                                                                                 cliente.Status,
                                                                                                 cliente.Email,
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
            var list = await _context.Connection.QueryAsync<ClienteTO>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Cpf = cpf },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
            
            return from cliente in list select ClienteRepositoryMapping.MapearClienteEndereco(cliente);
        }
        public async Task<IEnumerable<Cliente>> BuscarClientePorEmail(string email, CancellationToken token)
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
                        where Email = @Email";
            var list = await _context.Connection.QueryAsync<ClienteTO>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Email = email },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
            return from cliente in list select ClienteRepositoryMapping.MapearClienteEndereco(cliente);
        }

        public async Task<IEnumerable<Cliente>> BuscarClientePorId(string id, CancellationToken token)
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
                        where c.Id = @Id";
            var list = await _context.Connection.QueryAsync<ClienteTO>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Id = id },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));

            return from cliente in list select ClienteRepositoryMapping.MapearClienteEndereco(cliente);
        }

        public async Task<IEnumerable<Cliente>> BuscarClientePorNome(string nome, CancellationToken token)
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
                        where UPPER(Nome) like @Nome";
            var list = await _context.Connection.QueryAsync<ClienteTO>(new CommandDefinition(commandText: query,
                                                                                                    parameters: new { Nome = string.Concat("%", nome.ToUpper(), "%") },
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
            return from cliente in list select ClienteRepositoryMapping.MapearClienteEndereco(cliente);
        }

        public async Task<IEnumerable<Cliente>> BuscarClientes(CancellationToken token)
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
                        Join Endereco e on e.ClienteId = c.Id";
            var  list = await _context.Connection.QueryAsync<ClienteTO>(new CommandDefinition(commandText: query,
                                                                                                    transaction: _context.Transaction,
                                                                                                    commandType: System.Data.CommandType.Text,
                                                                                                    cancellationToken: token));
            return from cliente in list select ClienteRepositoryMapping.MapearClienteEndereco(cliente);
        }

        public async Task<int> CadastrarCliente(Cliente cliente, CancellationToken token)
        {
            var query = @"insert into Cliente(Id, Nome, Cpf, Email, Status) values (@Id, @Nome, @Cpf, @Email, 1)";
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
        public async Task<int> CadastrarEnderecoCliente(Cliente cliente, CancellationToken token)
        {
            var query = @"insert into Endereco(ClienteId, Rua, NumeroCasa, Complemento, CEP, Bairro, Cidade, Estado) 
                            values (@ClienteId, @Rua, @NumeroCasa, @Complemento, @CEP, @Bairro, @Cidade, @Estado)";

            return await _context.Connection.ExecuteAsync(new CommandDefinition(commandText: query,
                                                                                parameters: new
                                                                                {
                                                                                    ClienteId = cliente.Id,
                                                                                    cliente.Endereco.Rua,
                                                                                    cliente.Endereco.NumeroCasa,
                                                                                    cliente.Endereco.Complemento,
                                                                                    cliente.Endereco.CEP,
                                                                                    cliente.Endereco.Bairro,
                                                                                    cliente.Endereco.Cidade,
                                                                                    cliente.Endereco.Estado
                                                                                },
                                                                                transaction: _context.Transaction,
                                                                                commandType: System.Data.CommandType.Text,
                                                                                cancellationToken: token));
        }

        public async Task<int> AtualizarEnderecoCliente(Cliente cliente, CancellationToken token)
        {
            var query = @"update Endereco set Rua = @Rua, 
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
                                                                                    cliente.Endereco.Rua,
                                                                                    cliente.Endereco.NumeroCasa,
                                                                                    cliente.Endereco.Complemento,
                                                                                    cliente.Endereco.CEP,
                                                                                    cliente.Endereco.Bairro,
                                                                                    cliente.Endereco.Cidade,
                                                                                    cliente.Endereco.Estado
                                                                                },
                                                                                transaction: _context.Transaction,
                                                                                commandType: System.Data.CommandType.Text,
                                                                                cancellationToken: token));
        }
    }
}
