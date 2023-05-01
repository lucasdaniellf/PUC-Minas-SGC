using Clientes.Application.Events;
using Clientes.Domain;
using Clientes.Domain.Model;
using Clientes.Domain.Repository;
using Core.Infrastructure;
using Core.MessageBroker;
using Core.Messages.Commands;
using Core.Messages.Event;
using Microsoft.Extensions.Options;

namespace Clientes.Application.Commands
{
    public class ClienteCommandHandler : ICommandHandler<CadastrarClienteCommand, bool>,
                                         ICommandHandler<AtualizarClienteCommand, bool>,
                                         ICommandHandler<AtualizarStatusClienteCommand, bool>
    {

        private readonly IMessageBrokerPublisher _publisher;
        private readonly ClienteDomainSettings _settings;
        private readonly IClienteRepository _repository;
        private readonly IUnitOfWork<Cliente> _unitOfWork;

        public ClienteCommandHandler(IClienteRepository repository, IUnitOfWork<Cliente> unitOfWork, IMessageBrokerPublisher publisher, IOptions<ClienteDomainSettings> options)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
            _settings = options.Value;
        }

        public async Task<bool> Handle(CadastrarClienteCommand command, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                int row;

                var cpfCadastrado = (await _repository.BuscarClientePorCPF(command.Cpf, token)).Any();
                var emailCadastrado = (await _repository.BuscarClientePorEmail(command.Email, token)).Any();
                
                if (cpfCadastrado)
                {
                    throw new ClienteException("CPF já cadastrados no sistema para outro usuário");
                }
                else if (emailCadastrado)
                {
                    throw new ClienteException("Email já cadastrados no sistema para outro usuário");
                }
                else
                {
                    var cliente = Cliente.CadastrarCliente(command.Nome, command.Cpf, command.Email, command.Endereco);
                    row = await _repository.CadastrarCliente(cliente, token);
                    command.Id = cliente.Id;

                    if (row > 0)
                    {
                        await _repository.CadastrarEnderecoCliente(cliente, token);
                        EventRequest message = new ClienteMensagemEvent(cliente.Id, cliente.Email, cliente.EstaAtivo);
                        await Enqueue(_settings.FilaClienteCadastrado, message.Serialize());
                    }

                }
                _unitOfWork.CloseConnection();
                return row > 0;
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
        }

        public async Task<bool> Handle(AtualizarClienteCommand command, CancellationToken token)
        {
            int row = 0;
            try
            {
                _unitOfWork.Begin();
                IEnumerable<Cliente> clientes = await _repository.BuscarClientePorId(command.Id, token);

                if (clientes.Any())
                {
                    Cliente cliente = clientes.First();
                    cliente.AtualizarDadosCliente(command.Nome, command.Cpf, command.Email, command.Endereco);
                    
                    //cliente.AtualizarStatusCliente(command.EstaAtivo) ;

                    row = await _repository.AtualizarCliente(cliente, token);

                    if (row > 0)
                    {
                        await _repository.AtualizarEnderecoCliente(cliente, token);
                        EventRequest message = new ClienteMensagemEvent(cliente.Id, cliente.Email, cliente.EstaAtivo);
                        await Enqueue(_settings.FilaClienteAtualizado, message.Serialize());
                    }
                }
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
            return row > 0;
        }

        public async Task<bool> Handle(AtualizarStatusClienteCommand command, CancellationToken token)
        {
            int row = 0;
            try
            {
                _unitOfWork.Begin();
                IEnumerable<Cliente> clientes = await _repository.BuscarClientePorId(command.Id, token);

                if (clientes.Any())
                {
                    Cliente cliente = clientes.First();
                    cliente.AtualizarStatusCliente(command.EstaAtivo);
                    row = await _repository.AtualizarCliente(cliente, token);

                    if (row > 0)
                    {
                        EventRequest message = new ClienteMensagemEvent(cliente.Id, cliente.Email, cliente.EstaAtivo);
                        await Enqueue(_settings.FilaClienteAtualizado, message.Serialize());
                    }
                }
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
            return row > 0;
        }

        private async Task Enqueue(string queue, string message)
        {
            await _publisher.Enqueue(queue, message);

        }
    }
}
