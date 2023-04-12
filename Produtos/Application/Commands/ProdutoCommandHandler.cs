using Core.Infrastructure;
using Core.MessageBroker;
using Core.Messages.Commands;
using Microsoft.Extensions.Options;
using Produtos.Application.Commands.ProdutoEstoque;
using Produtos.Application.Events;
using Produtos.Domain;
using Produtos.Domain.Model;
using Produtos.Domain.Repository;
using static Produtos.Domain.Model.LogEstoque;

namespace Produtos.Application.Commands
{
    public class ProdutoCommandHandler : ICommandHandler<AdicionarProdutoEmCatalogoCommand, bool>,
                                           ICommandHandler<AtualizarCadastroProdutoCommand, bool>,
                                           ICommandHandler<CadastrarProdutoCommand, bool>,
                                           ICommandHandler<ReporEstoqueProdutoCommand, bool>,
                                           ICommandHandler<BaixarEstoqueProdutoCommand, bool>,
                                           ICommandHandler<RetirarProdutoDeCatalogoCommand, bool>
    {
        private readonly IUnitOfWork<Produto> _unitOfWork;
        private readonly IProdutoRepository _repository;
        private readonly IMessageBrokerPublisher _publisher;
        private readonly ProdutoDomainSettings _settings;

        public ProdutoCommandHandler(IUnitOfWork<Produto> unitOfWork, IProdutoRepository repository, IMessageBrokerPublisher publisher, IOptions<ProdutoDomainSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _publisher = publisher;
            _settings = settings.Value;
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
                    var eventRequest = new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, (int)produto.EstaAtivo);
                    await _publisher.Enqueue(_settings.FilaProdutoCadastrado, eventRequest.Serialize());
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
                    produto.Estoque.AtualizarEstoqueMinimo(command.EstoqueMinimo);

                    row = await _repository.AtualizarCadastroProduto(produto, token);
                    if (row > 0)
                    {
                        await _repository.AtualizarEstoqueMinimoProduto(produto.Estoque, token);
                        var eventRequest = new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, (int)produto.EstaAtivo);
                        await _publisher.Enqueue(_settings.FilaProdutoAtualizado, eventRequest.Serialize());
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

        public async Task<bool> Handle(AdicionarProdutoEmCatalogoCommand command, CancellationToken token)
        {
            int row = 0;
            try
            {
                _unitOfWork.Begin();
                var produtos = await _repository.BuscarProdutoPorId(command.Id, token);
                if (produtos.Any())
                {
                    var produto = produtos.First();
                    produto.AdicionarACatalogoDeVenda();

                    row = await _repository.AdicionarProdutoACatalogo(command.Id, token);
                    if (row > 0)
                    {
                        var eventRequest = new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, (int)produto.EstaAtivo);
                        await _publisher.Enqueue(_settings.FilaProdutoStatusAlterado, eventRequest.Serialize());
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

        public async Task<bool> Handle(RetirarProdutoDeCatalogoCommand command, CancellationToken token)
        {

            int row = 0;
            try
            {
                _unitOfWork.Begin();
                var produtos = await _repository.BuscarProdutoPorId(command.Id, token);
                if (produtos.Any())
                {
                    var produto = produtos.First();
                    produto.RetirarDeCatalogoDeVenda();

                    row = await _repository.RetirarProdutoDeCatalogo(command.Id, token);
                    if (row > 0)
                    {
                        var eventRequest = new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, (int)produto.EstaAtivo);
                        await _publisher.Enqueue(_settings.FilaProdutoStatusAlterado, eventRequest.Serialize());
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

        public async Task<bool> Handle(ReporEstoqueProdutoCommand command, CancellationToken token)
        {
            int rows = 0;
            _unitOfWork.Begin();
            _unitOfWork.BeginTransaction();

            var eventRequests = new List<ProdutoMensagemEvent>();
            try
            {
                foreach (var c in command.Produtos)
                {

                    var produtos = await _repository.BuscarProdutoPorIdBloquearRegistro(c.ProdutoId, token);
                    if (produtos.Any())
                    {
                        var produto = produtos.First();
                        produto.Estoque.AtualizarEstoque(produto.Estoque.Quantidade + c.Quantidade);

                        rows = await _repository.AtualizarQuantidadeEstoqueProduto(produto.Estoque, token);

                        if (rows > 0)
                        {
                            await GerarLogEstoque(produto.Estoque, token);
                            eventRequests.Add(new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, (int)produto.EstaAtivo));
                        }
                    }
                }

                _unitOfWork.Commit();
                foreach (var evento in eventRequests)
                {
                    await _publisher.Enqueue(_settings.FilaProdutoEstoqueAlterado, evento.Serialize());
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
                foreach (var c in command.Produtos)
                {
                    var produtos = await _repository.BuscarProdutoPorIdBloquearRegistro(c.ProdutoId, token);
                    if (produtos.Any())
                    {
                        var produto = produtos.First();
                        produto.Estoque.AtualizarEstoque(produto.Estoque.Quantidade - c.Quantidade);

                        var row = await _repository.AtualizarQuantidadeEstoqueProduto(produto.Estoque, token);
                        if (row > 0)
                        {
                            rows += row;
                            await GerarLogEstoque(produto.Estoque, token);
                            eventRequests.Add(new ProdutoMensagemEvent(produto.Id, produto.Preco, produto.Estoque.Quantidade, (int)produto.EstaAtivo));
                        }
                    }
                }

                _unitOfWork.Commit();
                foreach (var evento in eventRequests)
                {
                    await _publisher.Enqueue(_settings.FilaProdutoEstoqueAlterado, evento.Serialize());
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
