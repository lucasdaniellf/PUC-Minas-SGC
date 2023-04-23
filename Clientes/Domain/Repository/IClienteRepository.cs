using Clientes.Domain.Model;

namespace Clientes.Domain.Repository
{
    public interface IClienteRepository
    {
        public Task<IEnumerable<Cliente>> BuscarClientes(CancellationToken token);
        public Task<IEnumerable<Cliente>> BuscarClientePorNome(string nome, CancellationToken token);
        public Task<IEnumerable<Cliente>> BuscarClientePorCPF(string cpf, CancellationToken token); 
        public Task<IEnumerable<Cliente>> BuscarClientePorEmail(string email, CancellationToken token);
        public Task<IEnumerable<Cliente>> BuscarClientePorId(string id, CancellationToken token);
        public Task<int> CadastrarCliente(Cliente cliente, CancellationToken token);
        public Task<int> AtualizarCliente(Cliente cliente, CancellationToken token);
        public Task<int> AtualizarEnderecoCliente(Cliente cliente, CancellationToken token);
        public Task<int> CadastrarEnderecoCliente(Cliente cliente, CancellationToken token);

    }
}
