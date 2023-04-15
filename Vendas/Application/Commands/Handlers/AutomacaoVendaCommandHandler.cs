using Core.Infrastructure;
using Core.MessageBroker;
using Core.Messages.Commands;
using Vendas.Application.Commands.AutomacaoVendaCommands;
using Vendas.Application.Commands.Messages.Enviadas;
using Vendas.Domain.Model;

namespace Vendas.Application.Commands.Handlers
{
    public partial class VendaCommandHandler : ICommandHandler<AprovarVendaCommand, bool>,
                                                ICommandHandler<ReprovarVendaCommand, bool>,
                                                ICommandHandler<FaturarVendaCommand, bool>
    {
        public async Task<bool> Handle(AprovarVendaCommand command, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var venda = (await _repository.BuscarVendaPorId(command.VendaId, token)).First();
                venda.FinalizarVenda();
                var sucesso = await _repository.AtualizarVenda(venda, token) > 0;
                _unitOfWork.CloseConnection();
                return sucesso;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat(ex.Message, " - ", ex.StackTrace));
                _unitOfWork.CloseConnection();
                throw;
            }
        }

        public async Task<bool> Handle(ReprovarVendaCommand command, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var venda = (await _repository.BuscarVendaPorId(command.VendaId, token)).First();

                var statusIncial = venda.Status;

                venda.ReprovarVenda();
                var sucesso = await _repository.AtualizarVenda(venda, token) > 0;
                if (sucesso && statusIncial == StatusVenda.Status.AGUARDANDO_PAGAMENTO)
                {
                    var mensagem = GerarMensagemReposicaoProdutod(venda.Items);
                    await _publisher.Enqueue(_settings.FilaReporProduto, mensagem);
                }
                _unitOfWork.CloseConnection();
                return sucesso;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat(ex.Message, " - ", ex.StackTrace));
                _unitOfWork.CloseConnection();
                throw;
            }
        }

        public async Task<bool> Handle(FaturarVendaCommand command, CancellationToken token)
        {
            try
            {
                _unitOfWork.Begin();
                var venda = (await _repository.BuscarVendaPorId(command.VendaId, token)).First();
                venda.FaturarVenda();
                var sucesso = await _repository.AtualizarVenda(venda, token) > 0;

                if (sucesso)
                {
                    await _publisher.Enqueue(_settings.FilaGerarFatura, new GerarFaturaCommandMessage(command.VendaId).Serialize());
                }

                _unitOfWork.CloseConnection();
                return sucesso;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat(ex.Message, " - ", ex.StackTrace));
                _unitOfWork.CloseConnection();
                throw;
            }
        }
    }
}
