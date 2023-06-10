using Newtonsoft.Json;
using Produtos.Application.Commands.ProdutoEstoque;
using Produtos.Application.Commands;
using Produtos.Application.Commands.AutomacaoVendaCommands.Messages.Recebidas;
using Vendas.Application.Commands.Handlers;
using Vendas.Application.Commands.AutomacaoVendaCommands;
using Polly.Fallback;
using Polly;
using Polly.Retry;
using Polly.Wrap;

namespace AplicacaoGerenciamentoLoja.HostedServices.Consumers.Produto
{
    public class ReservarProdutoConsumer : BaseConsumer
    {
        public ReservarProdutoConsumer(IServiceProvider provider, IConfiguration configuration, ILogger<BaseConsumer> logger) : base(provider, configuration, logger)
        {
        }

        protected override string QueueName => _configuration.GetSection("Queues").GetSection("VendaDomainSettings")["FilaReservarProduto"];

        protected override async Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                ProdutoCommandHandler handler = scope.ServiceProvider.GetRequiredService<ProdutoCommandHandler>();
                VendaCommandHandler vHandler = scope.ServiceProvider.GetRequiredService<VendaCommandHandler>();
                
                foreach (var mensagem in mensagens)
                {
                    var sucessoReservaProduto = false;

                    await _wrapPolicy.ExecuteAsync( async (context) => {
                        var eventoDesserializado = JsonConvert.DeserializeObject<ReservarProdutoCommandMessage>(mensagem);
                        if (eventoDesserializado != null)
                        {
                            _logger.LogInformation("Dequeue: {mensagem}", eventoDesserializado.Serialize());
                            var comando = MapearEventoParaComando(eventoDesserializado.Produtos);
                            

                            await _wrapPolicy.ExecuteAsync(async (context) => {
                                sucessoReservaProduto = await handler.Handle(comando, token);
                            }, new Context()
                            {
                                ["mensagem"] = mensagem,
                                ["reservaProduto"] = sucessoReservaProduto
                            });

                            _logger.LogInformation("Retorno Reserva dos produtos: {VendaId} - {sucessoReservaProduto}", eventoDesserializado.VendaId, sucessoReservaProduto);

                            if (sucessoReservaProduto)
                            {
                                await vHandler.Handle(new FaturarVendaCommand(eventoDesserializado.VendaId), token);
                            }
                            else
                            {
                                await vHandler.Handle(new ReprovarVendaCommand(eventoDesserializado.VendaId), token);
                            }
                        }

                    }, new Context()
                    {
                        ["mensagem"] = mensagem,
                        ["reservaProduto"] = sucessoReservaProduto
                    });

                    //Console.WriteLine("ReservarProdutoVenda: " + mensagem);
                    //var eventoDesserializado = JsonConvert.DeserializeObject<ReservarProdutoCommandMessage>(mensagem);

                    //if (eventoDesserializado != null)
                    //{
                    //    var comando = MapearEventoParaComando(eventoDesserializado.Produtos);
                    //    if(await handler.Handle(comando, token))
                    //    {
                    //        await vHandler.Handle(new FaturarVendaCommand(eventoDesserializado.VendaId), token);
                    //    }
                    //    else
                    //    {
                    //        await vHandler.Handle(new ReprovarVendaCommand(eventoDesserializado.VendaId), token);
                    //    }
                    //}
                }
            }
        }


        private BaixarEstoqueProdutoCommand MapearEventoParaComando(IEnumerable<ProdutoVendaCommandMessage> produtosEvento)
        {
            IList<EstoqueProduto> produtos = new List<EstoqueProduto>();
            foreach (var produto in produtosEvento)
            {
                var p = new EstoqueProduto(produto.ProdutoId, produto.Quantidade);
                produtos.Add(p);
            }

            return new BaixarEstoqueProdutoCommand(produtos);
        }

        protected override Task QueueConsumerFallbackMethod(Context context, CancellationToken token)
        {

            if(context.TryGetValue("reservaProduto", out var sucessoReservaProduto))
            {
                Console.WriteLine($"Reserva de produto: {(bool) sucessoReservaProduto}");
            }

            return base.QueueConsumerFallbackMethod(context, token);
        }
    }
}
