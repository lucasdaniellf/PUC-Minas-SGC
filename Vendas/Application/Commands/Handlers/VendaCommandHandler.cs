using Core.Extensions;
using Core.Infrastructure;
using Core.MessageBroker;
using Core.Messages.Commands;
using Core.Messages.Event;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Vendas.Application.Commands.Messages.Enviadas;
using Vendas.Domain;
using Vendas.Domain.Model;
using Vendas.Domain.Repository;

namespace Vendas.Application.Commands.Handlers
{
    public partial class VendaCommandHandler : ICommandHandler<CriarVendaCommand, bool>,
                                         ICommandHandler<AtualizarFormaPagamentoVendaCommand, bool>,
                                         ICommandHandler<AplicarDescontoVendaCommand, bool>,
                                         ICommandHandler<CancelarVendaCommand, bool>,
                                         ICommandHandler<ConfirmarVendaCommand, bool>,
                                         ICommandHandler<AdicionarItemVendaCommand, bool>,
                                         ICommandHandler<RemoverItemVendaCommand, bool>,
                                         ICommandHandler<AtualizarItemVendaCommand, bool>
    {

        private readonly IUnitOfWork<Venda> _unitOfWork;
        private readonly IVendaRepository _repository;
        private readonly IMessageBrokerPublisher _publisher;
        private readonly VendaDomainSettings _settings;
        private readonly ILogger<VendaCommandHandler> _logger;
        public VendaCommandHandler(IUnitOfWork<Venda> unitOfWork, IVendaRepository repository, IMessageBrokerPublisher publisher, IOptions<VendaDomainSettings> settings, ILogger<VendaCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _publisher = publisher;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<bool> Handle(CriarVendaCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin();
            var clientes = await _repository.BuscarClientePorId(command.ClienteId, token);
            if (clientes.Any())
            {
                var cliente = clientes.First();
                var venda = Venda.CriarVenda(command.CriadoPor, cliente);
                command.Id = venda.Id;

                try
                {
                    row = await _repository.CadastrarVenda(venda, token);
                    if(row > 0)
                    {
                        _logger.LogInformation("CommandId: {MessageId} - Venda cadastrada: {venda}", command.MessageId, venda.SerializedObjectString());
                    }
                }
                catch (Exception)
                {
                    _unitOfWork.CloseConnection();
                    throw;
                }

            }
            _unitOfWork.CloseConnection();
            return row > 0;
        }

        public async Task<bool> Handle(AtualizarFormaPagamentoVendaCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin();
            var vendas = await _repository.BuscarVendaPorId(command.Id, token);
            if (vendas.Any())
            {
                var venda = vendas.First();
                venda.AtualizarFormaDePagamentoVenda(command.FormaDePagamento);
                try
                {
                    row = await _repository.AtualizarVenda(venda, token);
                    if (row > 0)
                    {
                        _logger.LogInformation("CommandId: {MessageId} - Venda atualizada: {venda}", command.MessageId, venda.SerializedObjectString());
                    }
                }
                catch (Exception)
                {
                    _unitOfWork.CloseConnection();
                    throw;
                }
            }
            _unitOfWork.CloseConnection();
            return row > 0;
        }

        public async Task<bool> Handle(AplicarDescontoVendaCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin();
            var vendas = await _repository.BuscarVendaPorId(command.Id, token);
            if (vendas.Any())
            {
                var venda = vendas.First();
                venda.AplicarDesconto(command.Desconto);
                try
                {
                    row = await _repository.AtualizarVenda(venda, token);
                    if (row > 0)
                    {
                        _logger.LogInformation("CommandId: {MessageId} - Venda atualizada: {venda}", command.MessageId, venda.SerializedObjectString());
                    }
                }
                catch (Exception)
                {
                    _unitOfWork.CloseConnection();
                    throw;
                }
            }
            _unitOfWork.CloseConnection();
            return row > 0;
        }

        public async Task<bool> Handle(CancelarVendaCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin();
            var vendas = await _repository.BuscarVendaPorId(command.Id, token);
            if (vendas.Any())
            {
                try
                {
                    var venda = vendas.First();
                    venda.CancelarVenda();

                    row = await _repository.AtualizarVenda(venda, token);
                    if (row > 0)
                    {
                        _logger.LogInformation("CommandId: {MessageId} - Venda cancelada: {venda}", command.MessageId, venda.Id);
                        var mensagem = GerarMensagemReposicaoProdutod(venda.Items);

                        _logger.LogInformation("Queue: {FilaReporProduto} - Enqueue: {mensagem}", _settings.FilaReporProduto, mensagem);
                        await _publisher.Enqueue(_settings.FilaReporProduto, mensagem);
                    }
                }
                catch (Exception)
                {
                    _unitOfWork.CloseConnection();
                    throw;
                }
            }
            _unitOfWork.CloseConnection();
            return row > 0;
        }

        public async Task<bool> Handle(AdicionarItemVendaCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin();
            var vendas = await _repository.BuscarVendaPorId(command.VendaId, token);
            var produtos = await _repository.BuscarProdutoPorId(command.ProdutoId, token);
            if (vendas.Any() && produtos.Any())
            {
                var item = ItemVenda.CriarItemVenda(vendas.First(), produtos.First(), command.Quantidade, produtos.First().Preco);
                var venda = vendas.First();
                venda.AdicionarItemAVenda(item);
                try
                {
                    row = await _repository.AdicionarProdutoEmVenda(item, token);
                    if(row > 0)
                    {
                        _logger.LogInformation("CommandId: {MessageId} - Produto Adicionado em Venda: {item}", command.MessageId, item.SerializedObjectString());
                        _logger.LogInformation("CommandId: {MessageId} - Venda atualizada: {venda}", command.MessageId, venda.SerializedObjectString());
                    }
                }
                catch (Exception)
                {
                    _unitOfWork.CloseConnection();
                    throw;

                }
            }
            _unitOfWork.CloseConnection();
            return row > 0;
        }

        public async Task<bool> Handle(RemoverItemVendaCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin();
            var vendas = await _repository.BuscarVendaPorId(command.VendaId, token);
            if (vendas.Any())
            {
                var venda = vendas.First();

                var itens = venda.RemoverItemVenda(command.ProdutoId);
                if (itens.Any())
                {

                    try
                    {
                        var item = itens.First();
                        row = await _repository.RemoverProdutoEmVenda(item, token);
                        if(row > 0)
                        {
                            _logger.LogInformation("CommandId: {MessageId} - Produto Removido de Venda: {item}", command.MessageId, item.SerializedObjectString());
                            _logger.LogInformation("CommandId: {MessageId} - Venda atualizada: {venda}", command.MessageId, venda.SerializedObjectString());
                        }
                    }
                    catch (Exception)
                    {
                        _unitOfWork.CloseConnection();
                        throw;

                    }
                }
            }
            _unitOfWork.CloseConnection();
            return row > 0;
        }

        public async Task<bool> Handle(AtualizarItemVendaCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin(true);
            var vendas = await _repository.BuscarVendaPorId(command.VendaId, token);
            if (vendas.Any())
            {
                var venda = vendas.First();

                var itens = venda.Items.Where(i => i.Produto.Id == command.ProdutoId).ToList();
                if (itens.Any())
                {
                    itens.First().AtualizarQuantidadeItemVenda(command.Quantidade);
                    try
                    {
                        row = await _repository.AtualizarProdutoEmVenda(itens.First(), token);
                        if(row > 0)
                        {
                            _logger.LogInformation("CommandId: {MessageId} - Produto Atualizado em Venda: {item}", command.MessageId, itens.First().SerializedObjectString());
                            _logger.LogInformation("CommandId: {MessageId} - Venda atualizada: {venda}", command.MessageId, venda.SerializedObjectString());
                        }                    
                    }
                    catch (Exception)
                    {
                        _unitOfWork.CloseConnection();
                        throw;

                    }
                }
            }
            _unitOfWork.CloseConnection();
            return row > 0;
        }

        public async Task<bool> Handle(ConfirmarVendaCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin();
            var vendas = await _repository.BuscarVendaPorId(command.Id, token);
            if (vendas.Any())
            {
                try
                {
                    var venda = vendas.First();
                    venda.ProcessarVenda();
                    row = await _repository.AtualizarVenda(venda, token);

                    if (row > 0)
                    {
                        _logger.LogInformation("CommandId: {MessageId} - Venda Confirmada: {venda}", command.MessageId, venda.SerializedObjectString());

                        var produtos = new List<ProdutoVendaCommandMessage>();
                        foreach (var item in venda.Items)
                        {
                            produtos.Add(new ProdutoVendaCommandMessage(item.Produto.Id, item.Quantidade));
                        }
                        var mensagem = new ReservarProdutoCommandMessage(venda.Id, produtos).Serialize();
                        await _publisher.Enqueue(_settings.FilaReservarProduto, mensagem);
                        _logger.LogInformation("Queue: {FilaReservarProduto} - Enqueue: {mensagem}", _settings.FilaReporProduto, mensagem);

                    }

                }
                catch (Exception)
                {
                    _unitOfWork.CloseConnection();
                    throw;

                }

            }
            _unitOfWork.CloseConnection();
            return row > 0;
        }

        private string GerarMensagemReposicaoProdutod(IList<ItemVenda> itens)
        {
            var produtos = new List<ProdutoVendaCommandMessage>();
            foreach (var item in itens)
            {
                produtos.Add(new ProdutoVendaCommandMessage(item.Produto.Id, item.Quantidade));
            }

            var mensagem = new ReporProdutoCommandMessage(produtos);
            return mensagem.Serialize();
        }
    }
}
