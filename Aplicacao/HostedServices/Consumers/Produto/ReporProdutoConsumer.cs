using Newtonsoft.Json;
using Core.MessageBroker;
using Produtos.Application.Commands.ProdutoEstoque;
using Produtos.Application.Commands;
using Produtos.Application.Commands.AutomacaoVendaCommands.Messages.Recebidas;
using Polly;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Produto
{
    public class ReporProdutoConsumer : BaseConsumer
    {
        public ReporProdutoConsumer(IServiceProvider provider, IConfiguration configuration) : base(provider, configuration)
        {
        }

        protected override string QueueName => _configuration.GetSection("Queues").GetSection("VendaDomainSettings")["FilaReporProduto"];

        protected override async Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                ProdutoCommandHandler handler = scope.ServiceProvider.GetRequiredService<ProdutoCommandHandler>();
                IMessageBrokerPublisher publisher = scope.ServiceProvider.GetRequiredService<IMessageBrokerPublisher>();

                foreach (var mensagem in mensagens)
                {
                    await _wrapPolicy.ExecuteAsync(async (context) =>
                    {
                        Console.WriteLine("ReporProdutoVenda: " + mensagem);
                        var eventoDesserializado = JsonConvert.DeserializeObject<ReporProdutoCommandMessage>(mensagem);

                        if (eventoDesserializado != null)
                        {
                            var comando = MapearEventoParaComando(eventoDesserializado.Produtos);
                            await handler.Handle(comando, token);
                        }
                    }, new Context()
                    {
                        ["mensagem"] = mensagem
                    });
                }
            }
        }


        private ReporEstoqueProdutoCommand MapearEventoParaComando(IEnumerable<ProdutoVendaCommandMessage> produtosEvento)
        {
            IList<EstoqueProduto> produtos = new List<EstoqueProduto>();
            foreach (var produto in produtosEvento)
            {
                var p = new EstoqueProduto()
                {
                    ProdutoId = produto.ProdutoId,
                    Quantidade = produto.Quantidade
                };
                produtos.Add(p);
            }

            return new ReporEstoqueProdutoCommand()
            {
                Produtos = produtos,
            };
        }
    }
}
