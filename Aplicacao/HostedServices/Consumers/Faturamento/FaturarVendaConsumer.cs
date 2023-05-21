using Core.MessageBroker;
using Newtonsoft.Json;
using Polly;
using Vendas.Application.Commands.Messages.Enviadas;
using Vendas.Application.Commands.Messages.Recebidas;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Faturamento
{
    public class FaturarVendaConsumer : BaseConsumer
    {
        public FaturarVendaConsumer(IServiceProvider provider, IConfiguration configuration,ILogger<BaseConsumer> logger) : base(provider, configuration, logger)
        {
        }

        protected override string QueueName => _configuration.GetSection("Queues").GetSection("VendaDomainSettings")["FilaGerarFatura"];
        private string FilaFaturarVendaCallback => _configuration.GetSection("Queues").GetSection("VendaDomainSettings")["FilaFaturamentoCallback"];

        protected override async Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                IMessageBrokerPublisher publisher = scope.ServiceProvider.GetRequiredService<IMessageBrokerPublisher>();
                foreach(var mensagem in mensagens)
                {
                    await _wrapPolicy.ExecuteAsync(async (context) => {
                        var mensagemDesserializada = JsonConvert.DeserializeObject<GerarFaturaCommandMessage>(mensagem);
                        if (mensagemDesserializada != null)
                        {
                            //Doing this way becaus when a new command/event is created/desserialized, a new correlationId is created.
                            //this approach logs the new command, with the new command Id, which in turn can be used to track the command in commandHandler
                            _logger.LogInformation("Dequeue: {mensagem}", mensagemDesserializada.Serialize());

                            Random rndm = new();
                            int value = rndm.Next(0, 9);
                            bool success = false;
                            if (value > 5)
                            {
                                success = true;
                            }

                            _logger.LogInformation("Venda {VendaId}  pagamento aprovado: {success}", mensagemDesserializada.VendaId, success);

                            var mensagemConfirmacao = new FaturarVendaCallback(mensagemDesserializada.VendaId, success).Serialize();
                            await publisher.Enqueue(FilaFaturarVendaCallback, mensagemConfirmacao);
                        }
                    }, new Context() { 
                        ["mensagem"] = mensagem 
                    });
                }
            }
        }
    }
}
