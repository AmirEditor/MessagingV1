using AmirMahdiMousazadeh;
using Client2.Extentions;
using Grpc.Net.Client;

namespace Client2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private Messenger.MessengerClient _client = null;
        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        protected Messenger.MessengerClient Client
        {
            get
            {
                if (_client == null)
                {
                    var opt = new GrpcChannelOptions
                    {
                        //  LoggerFactory = _loggerFactory
                    };
                    string url = _configuration["Service:ServerUrl"];
                    var channel = GrpcChannel.ForAddress(url);
                    _client = new Messenger.MessengerClient(channel);
                }

                return _client;
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var res = Client.SubscribeMessages(new UserInfo { Name = "Roya" });

                var theTask = Task.Run(async () =>
                {
                    await foreach (var reading in res.ResponseStream.ToAsyncEnumerable())
                    {
                        _logger.LogInformation($"received data : {reading.Text}");
                    }
                });

                await theTask;
                await Task.Delay(3000);
            }
        }
    }
}