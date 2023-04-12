using Clientes.Domain.Model;

namespace Clientes.Domain.Repository
{
    public interface IClienteRepository
    {
        public Task<IEnumerable<Cliente>> BuscarClientes(CancellationToken token);
        public Task<IEnumerable<Cliente>> BuscarClientePorNome(string nome, CancellationToken token);
        public Task<IEnumerable<Cliente>> BuscarClientePorCPF(string cpf, CancellationToken token);
        public Task<IEnumerable<Cliente>> BuscarClientePorId(string id, CancellationToken token);
        public Task<int> CadastrarCliente(Cliente cliente, CancellationToken token);
        public Task<int> AtualizarCadastroCliente(Cliente cliente, CancellationToken token);
        public Task<int> AtivarCliente(string id, CancellationToken token);
        public Task<int> InativarCliente(string id, CancellationToken token);
    }
}
