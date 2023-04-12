using Core.MessageBroker;
using Newtonsoft.Json;
using Vendas.Application.Commands.AutomacaoVendaCommands;
using Vendas.Application.Commands.Handlers;
using Vendas.Application.Commands.Messages.Recebidas;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Venda
{
    public class FaturamentoCallbackConsumer : BaseConsumer
    {
        public FaturamentoCallbackConsumer(IServiceProvider provider, IConfiguration configuration) : base(provider, configuration)
        {
        }

        protected override string QueueName => _configuration.GetSection("Queues").GetSection("VendaDomainSettings")["FilaFaturamentoCallback"];

        protected override async Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                VendaCommandHandler handler = scope.ServiceProvider.GetRequiredService<VendaCommandHandler>();
                IMessageBrokerPublisher publisher = scope.ServiceProvider.GetRequiredService<IMessageBrokerPublisher>();

                foreach (var mensagem in mensagens)
                {
                    Console.WriteLine("FaturarVendaCallback: " + mensagem);
                    var eventoDesserializado = JsonConvert.DeserializeObject<FaturarVendaCallback>(mensagem);

                    if (eventoDesserializado != null)
                    {
                        if (eventoDesserializado.Sucesso)
                        {
                            await handler.Handle(new AprovarVendaCommand(eventoDesserializado.VendaId), token);
                        }
                        else
                        {
                            await handler.Handle(new ReprovarVendaCommand(eventoDesserializado.VendaId), token);
                        }
                    }
                }
            }
        }
    }
}
