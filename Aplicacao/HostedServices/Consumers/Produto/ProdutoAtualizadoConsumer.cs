using Newtonsoft.Json;
using Vendas.Application.Events;
using Vendas.Application.Events.Produto;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Produto
{
    public class ProdutoAtualizadoConsumer : BaseConsumer
    {

        public ProdutoAtualizadoConsumer(IServiceProvider provider,
                                        IConfiguration configuration) : base(provider, configuration) { }

        protected override string QueueName => "produto-*";

        protected override async Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                VendaEventHandler handler = scope.ServiceProvider.GetRequiredService<VendaEventHandler>();
                foreach (var mensagem in mensagens)
                {
                    Console.WriteLine("EventoProdutoAtualizado: " + mensagem);
                    var deserialized = JsonConvert.DeserializeObject<ProdutoVendaAtualizadoEvent>(mensagem);
                    if (deserialized != null)
                    {
                        await handler.Handle(deserialized, token);
                    }
                }
            }
        }
    }
}
