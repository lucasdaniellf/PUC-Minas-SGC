using Core.Infrastructure;
using Core.MessageBroker;
using Core.Messages.Commands;
using Core.Messages.Event;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Produtos.Application.Commands.ProdutoEstoque;
using Produtos.Application.Events;
using Produtos.Domain;
using Produtos.Domain.Model;
using Produtos.Domain.Repository;
using static Produtos.Domain.Model.LogEstoque;

namespace Produtos.Application.Commands
{
    public class ProdutoCommandHandler :   ICommandHandler<AtualizarCadastroProdutoCommand, bool>,
                                           ICommandHandler<CadastrarProdutoCommand, bool>,
                                           ICommandHandler<ReporEstoqueProdutoCommand, bool>,
                                           ICommandHandler<BaixarEstoqueProdutoCommand, bool>
    {
        private readonly IUnitOfWork<Produto> _unitOfWork;
        private readonly IProdutoRepository _repository;
        private readonly IMessageBrokerPublisher _publisher;
        private readonly ProdutoDomainSettings _settings;
        private readonly ILogger<ProdutoCommandHandler> _logger;
        public ProdutoCommandHandler(IUnitOfWork<Produto> unitOfWork, IProdutoRepository repository, IMessageBrokerPublisher publisher, IOptions<ProdutoDomainSettings> settings, ILogger<ProdutoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _publisher = publisher;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<bool> Handle(CadastrarProdutoCommand command, CancellationToken token)
        {
            int row = 0;
            _unitOfWork.Begin();
            _unitOfWork.BeginTransaction();
            try
            {
                Produto produto = Produto.CadastrarProduto(command.Descricao, command.Preco);
                row = await _repository.CadastrarProduto(produto, token);
                if (row > 0)
                {
                    var eventRequest = new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, produto.EstaAtivo);
                    
                    string eventSerialized = eventRequest.Serialize();
                    _logger.LogInformation("Queue: {FilaProdutoCadastrado} - Enqueue: {eventRequest}", _settings.FilaProdutoCadastrado, eventSerialized);
                    await _publisher.Enqueue(_settings.FilaProdutoCadastrado, eventSerialized);
                    command.Id = produto.Id;
                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;

            }
            return row > 0;
        }

        public async Task<bool> Handle(AtualizarCadastroProdutoCommand command, CancellationToken token)
        {

            int row = 0;
            try
            {
                _unitOfWork.Begin();
                _unitOfWork.BeginTransaction();
                var produtos = await _repository.BuscarProdutoPorId(command.Id, token);
                if (produtos.Any())
                {
                    var produto = produtos.First();
                    produto.AtualizarDescricao(command.Descricao);
                    produto.AtualizarPreco(command.Preco);
                    produto.AtualizarStatusProduto(command.EstaAtivo);
                    produto.Estoque.AtualizarEstoqueMinimo(command.EstoqueMinimo);

                    row = await _repository.AtualizarCadastroProduto(produto, token);
                    if (row > 0)
                    {
                        await _repository.AtualizarEstoqueMinimoProduto(produto.Estoque, token);
                        var eventRequest = new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, produto.EstaAtivo);

                        string eventSerialized = eventRequest.Serialize();
                        _logger.LogInformation("Queue: {FilaProdutoAtualizado} - Enqueue: {eventRequest}", _settings.FilaProdutoAtualizado, eventSerialized);
                        await _publisher.Enqueue(_settings.FilaProdutoAtualizado, eventSerialized);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
            return row > 0;
        }

        public async Task<bool> Handle(ReporEstoqueProdutoCommand command, CancellationToken token)
        {
            int rows = 0;
            _unitOfWork.Begin();
            _unitOfWork.BeginTransaction();

            var eventRequests = new List<ProdutoMensagemEvent>();
            try
            {
                _logger.LogInformation("CommandId: {MessageId} - Inicio reposição de estoque", command.MessageId);

                foreach (var c in command.Produtos)
                {
                    _logger.LogInformation("CommandId: {MessageId} - reposição de estoque para produto: {ProdutoId}", command.MessageId, c.ProdutoId);
                    var produtos = await _repository.BuscarProdutoPorIdBloquearRegistro(c.ProdutoId, token);
                    if (produtos.Any())
                    {
                        var produto = produtos.First();
                        produto.ReporEstoque(c.Quantidade);

                        rows = await _repository.AtualizarQuantidadeEstoqueProduto(produto.Estoque, token);

                        if (rows > 0)
                        {
                            _logger.LogInformation("CommandId: {MessageId} - Processo de reposição de estoque: {ProdutoId} - Nova Quantidade: {NovaQuantidade}",command.MessageId, c.ProdutoId, produto.Estoque.Quantidade);
                            await GerarLogEstoque(produto.Estoque, token);
                            eventRequests.Add(new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, produto.EstaAtivo));
                        }
                    }
                }

                _logger.LogInformation("CommandId: {MessageId} - Fim reposição de estoque", command.MessageId);
                _unitOfWork.Commit();
                foreach (var evento in eventRequests)
                {
                    string eventSerialized = evento.Serialize();
                    _logger.LogInformation("Queue: {FilaProdutoEstoqueAlterado} - Enqueue: {eventSerialized}", _settings.FilaProdutoAtualizado, eventSerialized);
                    await _publisher.Enqueue(_settings.FilaProdutoEstoqueAlterado, eventSerialized);
                }
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
            return rows > 0;
        }


        //Seria melhor enviar um evento de atualização do delta do estoque?
        public async Task<bool> Handle(BaixarEstoqueProdutoCommand command, CancellationToken token)
        {
            int rows = 0;
            _unitOfWork.Begin();
            _unitOfWork.BeginTransaction();

            var eventRequests = new List<ProdutoMensagemEvent>();
            try
            {
                _logger.LogInformation("CommandId: {MessageId} - Inicio baixa de estoque", command.MessageId);
                foreach (var c in command.Produtos)
                {
                    _logger.LogInformation("CommandId: {MessageId} - baixa de estoque para produto: {ProdutoId}", command.MessageId, c.ProdutoId);
                    var produtos = await _repository.BuscarProdutoPorIdBloquearRegistro(c.ProdutoId, token);
                    if (produtos.Any())
                    {
                        var produto = produtos.First();
                        produto.BaixarEstoque(c.Quantidade);

                        var row = await _repository.AtualizarQuantidadeEstoqueProduto(produto.Estoque, token);
                        if (row > 0)
                        {
                            _logger.LogInformation("CommandId: {MessageId} - Processo de baixa de estoque: {ProdutoId} - Nova Quantidade: {NovaQuantidade}", command.MessageId, c.ProdutoId, produto.Estoque.Quantidade);
                            rows += row;
                            await GerarLogEstoque(produto.Estoque, token);
                            eventRequests.Add(new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, produto.EstaAtivo));
                        }
                    }
                }
                _logger.LogInformation("CommandId: {MessageId} - Fim baixa de estoque", command.MessageId);
                _unitOfWork.Commit();
                foreach (var evento in eventRequests)
                {
                    string eventSerialized = evento.Serialize();
                    _logger.LogInformation("Queue: {FilaProdutoEstoqueAlterado} - Enqueue: {eventRequests}", _settings.FilaProdutoAtualizado, eventSerialized);
                    await _publisher.Enqueue(_settings.FilaProdutoEstoqueAlterado, eventSerialized);
                }
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
            return rows > 0;
        }

        private async Task GerarLogEstoque(Estoque estoque, CancellationToken token)
        {
            var log = estoque.GerarLog();
            await _repository.GerarLogEstoque(log, token);
        }
    }
}
