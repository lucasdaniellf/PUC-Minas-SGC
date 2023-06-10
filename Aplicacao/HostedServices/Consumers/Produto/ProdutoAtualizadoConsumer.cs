using Newtonsoft.Json;
using Polly;
using Vendas.Application.Events;
using Vendas.Application.Events.Produto;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Produto
{
    public class ProdutoAtualizadoConsumer : BaseConsumer
    {

        public ProdutoAtualizadoConsumer(IServiceProvider provider,
                                        IConfiguration configuration, ILogger<BaseConsumer> logger) : base(provider, configuration, logger) { }

        protected override string QueueName => "produto-*";

        protected override async Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                VendaEventHandler handler = scope.ServiceProvider.GetRequiredService<VendaEventHandler>();
                foreach (var mensagem in mensagens)
                {
                    await _wrapPolicy.ExecuteAsync(async (context) =>
                    {
                        var deserialized = JsonConvert.DeserializeObject<ProdutoVendaAtualizadoEvent>(mensagem);
                        if (deserialized != null)
                        {
                            _logger.LogInformation("Dequeue: {mensagem}", deserialized.Serialize());
                            await handler.Handle(deserialized, token);
                        }
                    }, new Context()
                    {
                        ["mensagem"] = mensagem
                    });
                }
            }
        }
    }
}
