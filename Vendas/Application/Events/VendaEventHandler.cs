using Core.Extensions;
using Core.Infrastructure;
using Core.Messages.Event;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Vendas.Application.Events.Cliente;
using Vendas.Application.Events.Produto;
using Vendas.Domain.Model;
using Vendas.Domain.Repository;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Application.Events
{
    public class VendaEventHandler : IEventHandler<ClienteVendaAtualizadoEvent>,
                                     IEventHandler<ProdutoVendaAtualizadoEvent>
    {
        private readonly IUnitOfWork<Venda> _unitOfWork;
        private readonly IVendaRepository _repository;
        private readonly ILogger<VendaEventHandler> _logger;
        public VendaEventHandler(IUnitOfWork<Venda> unitOfWork, IVendaRepository repository, ILogger<VendaEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(ClienteVendaAtualizadoEvent request, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var cliente = new ClienteVenda(request.Id, request.Email, request.EstaAtivo);

                var clientes = await _repository.BuscarClientePorId(request.Id, token);
                if (clientes.Any())
                {
                    await _repository.AtualizarCliente(cliente, token);
                    _logger.LogInformation("EventMessageId: {messageId} - Cliente Atualizado: {cliente}", request.MessageId, cliente.SerializedObjectString());
                }
                else
                {
                    await _repository.CadastrarCliente(cliente, token);
                    _logger.LogInformation("EventMessageId: {messageId} - Cliente Cadastrado: {cliente}", request.MessageId, cliente.SerializedObjectString());
                }
            }
            catch (Exception)
            {
                _unitOfWork.CloseConnection();
                throw;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------//
        public async Task Handle(ProdutoVendaAtualizadoEvent request, CancellationToken token)
        {
            var produto = new ProdutoVenda(request.Id, request.Preco, request.QuantidadeEstoque, request.EstaAtivo);
            try
            {
                _unitOfWork.Begin();
                _unitOfWork.BeginTransaction();
                var produtos = await _repository.BuscarProdutoPorId(request.Id, token);
                if (produtos.Any())
                {
                    await _repository.AtualizarCadastroProduto(produto, token);

                    if (produtos.First().Preco != produto.Preco)
                    {
                        await AtualizarPrecoProdutosEmVendas(produto, token);
                    }
                    _logger.LogInformation("EventMessageId: {messageId} - Produto atualizado: {produto}", request.MessageId, produto.SerializedObjectString());
                }
                else
                {
                    await _repository.CadastrarProduto(produto, token);
                    _logger.LogInformation("EventMessageId: {messageId} - Produto cadastrado: {produto}", request.MessageId, produto.SerializedObjectString());
                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private async Task AtualizarPrecoProdutosEmVendas(ProdutoVenda produto, CancellationToken token)
        {
            var vendas = (await _repository.BuscarVendaPorStatusVenda(Status.PENDENTE, token)).Where(v => v.Items.Any(i => i.Produto.Id == produto.Id));
            foreach (var venda in vendas)
            {
                var selectedItem = from item in venda.Items where item.Produto.Id == produto.Id select item;
                if (selectedItem.Any())
                {
                    var item = selectedItem.First();
                    item.AtualizarValorPago(produto.Preco);
                    await _repository.AtualizarProdutoEmVenda(selectedItem.First(), token);
                }
            }
        }
    }
}
