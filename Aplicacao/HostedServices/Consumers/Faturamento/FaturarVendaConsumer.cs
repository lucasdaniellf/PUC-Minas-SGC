using Core.MessageBroker;
using Newtonsoft.Json;
using Polly;
using Vendas.Application.Commands.Messages.Enviadas;
using Vendas.Application.Commands.Messages.Recebidas;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Faturamento
{
    public class FaturarVendaConsumer : BaseConsumer
    {
        public FaturarVendaConsumer(IServiceProvider provider, IConfiguration configuration) : base(provider, configuration)
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
                            Random rndm = new();
                            int value = rndm.Next(0, 9);
                            bool success = false;
                            if (value > 5)
                            {
                                success = true;
                            }
                            Console.WriteLine($"Venda: {mensagemDesserializada.VendaId}; Pagamento: {success}");
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
