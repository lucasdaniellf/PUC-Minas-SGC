using Core.MessageBroker;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Polly.Wrap;

namespace AplicacaoGerenciamentoLoja.HostedServices
{
    public abstract class BaseConsumer : BackgroundService
    {
        protected readonly IServiceProvider _provider;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<BaseConsumer> _logger;

        protected readonly AsyncRetryPolicy _retryPolicy;
        protected readonly AsyncFallbackPolicy _fallbackPolicy;
        protected readonly AsyncPolicyWrap _wrapPolicy;
        protected abstract string QueueName { get; }

        public BaseConsumer(IServiceProvider provider,
                            IConfiguration configuration,
                            ILogger<BaseConsumer> logger)
        {
            _provider = provider;
            _configuration = configuration;
            _logger = logger;

            _retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(3, (attempt) => TimeSpan.FromSeconds(Math.Pow(attempt, 2)));
            _fallbackPolicy = Policy.Handle<Exception>().FallbackAsync(
                fallbackAction: QueueConsumerFallbackMethod,
                onFallbackAsync: async (exception, ctx) =>
                {
                    if (ctx.TryGetValue("mensagem", out var mensagem))
                    {
                        _logger.LogError("Erro ao realizar leitura de mensagem: {mensagem}", mensagem);
                    }

                    _logger.LogError(exception, "StackTrace: ");
                    await Task.CompletedTask;
                });
            _wrapPolicy = Policy.WrapAsync(_fallbackPolicy, _retryPolicy);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope subscriberScope = _provider.CreateScope())
            {
                var rand = new Random();
                int iter = 0;
                var subscriber = subscriberScope.ServiceProvider.GetRequiredService<IMessageBrokerSubscriber>();
                subscriber.Subscribe(QueueName);

                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Reading Queue Consumer {QueueName} - iteration {iter}");
                    var mensagens = subscriber.Dequeue();
                    await ProcessarMensagens(mensagens, stoppingToken);

                    iter++;
                    await Task.Delay(TimeSpan.FromSeconds(10 + rand.Next(1, 5)), stoppingToken);
                }
            }
        }

        protected abstract Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token); 
        protected virtual async Task QueueConsumerFallbackMethod(Context context, CancellationToken token)
        {
            await Task.CompletedTask;
        }
    }
}
