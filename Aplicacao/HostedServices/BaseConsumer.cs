using Core.MessageBroker;

namespace AplicacaoGerenciamentoLoja.HostedServices
{
    public abstract class BaseConsumer : BackgroundService
    {
        protected readonly IServiceProvider _provider;
        protected readonly IConfiguration _configuration;

        protected abstract string QueueName { get; }

        public BaseConsumer(IServiceProvider provider,
                            IConfiguration configuration)
        {
            _provider = provider;
            _configuration = configuration;
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
                    try
                    {
                        await ProcessarMensagens(mensagens, stoppingToken);
                    }catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    iter++;
                    await Task.Delay(TimeSpan.FromSeconds(10 + rand.Next(1, 5)), stoppingToken);
                }
            }
        }

        protected abstract Task ProcessarMensagens(IEnumerable<string> mensagens, CancellationToken token);
    }
}
