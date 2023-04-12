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
    public class ClienteCommandHandler : ICommandHandler<AtivarClienteCommand, bool>,
                                         ICommandHandler<InativarClienteCommand, bool>,
                                         ICommandHandler<CadastrarClienteCommand, bool>,
                                         ICommandHandler<AtualizarCadastroClienteCommand, bool>
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
            int row = 0;

            try
            {
                _unitOfWork.Begin();
                IEnumerable<Cliente> clientes = await _repository.BuscarClientePorCPF(command.Cpf, token);
                if (!clientes.Any())
                {
                    var cliente = Cliente.CadastrarCliente(command.Nome, command.Cpf);
                    row = await _repository.CadastrarCliente(cliente, token);
                    command.Id = cliente.Id;

                    if (row > 0)
                    {
                        EventRequest message = new ClienteMensagemEvent(cliente.Id, (int)cliente.EstaAtivo);
                        await Enqueue(_settings.FilaClienteCadastrado, message.Serialize());

                    }
                }
                _unitOfWork.CloseConnection();
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }

            return row > 0;
        }

        public async Task<bool> Handle(AtualizarCadastroClienteCommand command, CancellationToken token)
        {
            int row = 0;
            try
            {
                _unitOfWork.Begin();
                IEnumerable<Cliente> clientes = await _repository.BuscarClientePorId(command.Id, token);

                if (clientes.Any())
                {
                    Cliente cliente = clientes.First();
                    cliente.AtualizarCpf(command.Cpf);
                    cliente.AtualizarNome(command.Nome);
                    row = await _repository.AtualizarCadastroCliente(cliente, token);

                    if (row > 0)
                    {
                        EventRequest message = new ClienteMensagemEvent(cliente.Id, (int)cliente.EstaAtivo);
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

        public async Task<bool> Handle(AtivarClienteCommand command, CancellationToken token)
        {
            int row = 0;
            try
            {

                _unitOfWork.Begin();
                row = await _repository.AtivarCliente(command.Id, token);
                if (row > 0)
                {
                    var cliente = (await _repository.BuscarClientePorId(command.Id, token)).First();
                    EventRequest message = new ClienteMensagemEvent(cliente.Id, (int)cliente.EstaAtivo);
                    await Enqueue(_settings.FilaClienteStatusAlterado, message.Serialize());
                }
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
            return row > 0;
        }

        public async Task<bool> Handle(InativarClienteCommand command, CancellationToken token)
        {
            int row = 0;
            try
            {
                _unitOfWork.Begin();
                row = await _repository.InativarCliente(command.Id, token);

                if (row > 0)
                {
                    var cliente = (await _repository.BuscarClientePorId(command.Id, token)).First();
                    EventRequest message = new ClienteMensagemEvent(cliente.Id, (int)cliente.EstaAtivo);
                    await Enqueue(_settings.FilaClienteStatusAlterado, message.Serialize());
                }
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
            return row > 0;
        }

        //Single Responsibility?

        private async Task Enqueue(string queue, string message)
        {
            await _publisher.Enqueue(queue, message);

        }
    }
}
