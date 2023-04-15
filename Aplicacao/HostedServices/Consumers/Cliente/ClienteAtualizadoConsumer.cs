using Newtonsoft.Json;
using Vendas.Application.Events;
using Vendas.Application.Events.Cliente;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Cliente
{
    public class ClienteAtualizadoConsumer : BaseConsumer
    {
        public ClienteAtualizadoConsumer(IServiceProvider provider,
                                         IConfiguration configuration) : base(provider, configuration) { }


        protected override string QueueName
        {
            get
            {
                return "cliente-*";
            }
        }


        protected async override Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                VendaEventHandler handler = scope.ServiceProvider.GetRequiredService<VendaEventHandler>();
                foreach (var mensagem in mensagens)
                {
                    Console.WriteLine("EventoClienteAtualizado: " + mensagem);
                    var deserialized = JsonConvert.DeserializeObject<ClienteVendaAtualizadoEvent>(mensagem);
                    if (deserialized != null)
                    {
                        await handler.Handle(deserialized, token);
                    }
                }
            }
        }
    }
}
