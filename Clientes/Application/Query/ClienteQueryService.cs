using Clientes.Application.Query.DTO;
using Clientes.Domain.Model;
using Clientes.Domain.Repository;
using Core.Infrastructure;

namespace Clientes.Application.Query
{
    public class ClienteQueryService
    {
        private readonly IClienteRepository _repository;
        private readonly IUnitOfWork<Cliente> _unitOfWork;
        public ClienteQueryService(IClienteRepository repository, IUnitOfWork<Cliente> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<ClienteQueryDto>> BuscarClientes(CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var clientes = from c in await _repository.BuscarClientes(token) select MapQueryDto(c);

                _unitOfWork.CloseConnection();
                return clientes;
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
        }
        public async Task<IEnumerable<ClienteQueryDto>> BuscarClientePorNome(string nome, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var clientes = from c in await _repository.BuscarClientePorNome(nome, token) select MapQueryDto(c);

                _unitOfWork.CloseConnection();
                return clientes;
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
        }
        public async Task<IEnumerable<ClienteQueryDto>> BuscarClientePorCPF(string cpf, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var clientes = from c in await _repository.BuscarClientePorCPF(cpf, token) select MapQueryDto(c);

                _unitOfWork.CloseConnection();
                return clientes;
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }

        }

        public async Task<IEnumerable<ClienteQueryDto>> BuscarClientePorEmail(string email, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var clientes = from c in await _repository.BuscarClientePorEmail(email, token) select MapQueryDto(c);

                _unitOfWork.CloseConnection();
                return clientes;
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }

        }

        public async Task<IEnumerable<ClienteQueryDto>> BuscarClientePorId(string Id, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var clientes = from c in await _repository.BuscarClientePorId(Id, token) select MapQueryDto(c);

                _unitOfWork.CloseConnection();
                return clientes;
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }

        }

        private ClienteQueryDto MapQueryDto(Cliente cliente)
        {
            return new ClienteQueryDto(cliente.Id, cliente.Cpf.Numero, cliente.Nome, cliente.Email, cliente.EstaAtivo);
        }
    }
}
