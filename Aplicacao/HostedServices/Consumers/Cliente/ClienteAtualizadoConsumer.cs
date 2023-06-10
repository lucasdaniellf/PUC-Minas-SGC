using Newtonsoft.Json;
using Polly;
using Vendas.Application.Events;
using Vendas.Application.Events.Cliente;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Cliente
{
    public class ClienteAtualizadoConsumer : BaseConsumer
    {
        public ClienteAtualizadoConsumer(IServiceProvider provider,
                                         IConfiguration configuration, ILogger<BaseConsumer> logger) : base(provider, configuration, logger) {
        }
        protected override string QueueName => "cliente-*";
        protected async override Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                VendaEventHandler handler = scope.ServiceProvider.GetRequiredService<VendaEventHandler>();
                foreach (var mensagem in mensagens)
                {
                    await _wrapPolicy.ExecuteAsync( async (context) =>
                    {
                        var deserialized = JsonConvert.DeserializeObject<ClienteVendaAtualizadoEvent>(mensagem);
                        if (deserialized != null)
                        {
                            _logger.LogInformation("Dequeue: {mensagem}", deserialized.Serialize());
                            await handler.Handle(deserialized, token);
                        }
                    },
                    new Context()
                    {
                        ["mensagem"] = mensagem
                    });

                }
            }
        }
    }
}
