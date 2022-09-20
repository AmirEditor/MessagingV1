using AmirMahdiMousazadeh;
using Client.Extentions;
using Grpc.Net.Client;

namespace Client
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private string _userName = "";
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

            Console.WriteLine("please enter your name :");
            _userName = Console.ReadLine();
            // subscribe (asynchronous)
            var consoleLock = new object();
            _ = Client.SubscribeMessages(new UserInfo { Name = _userName }).ResponseStream.ToAsyncEnumerable()
                .ForEachAsync((x) =>
                {
                    // if the user is writing something, wait until it finishes.
                    lock (consoleLock)
                    {
                        Console.WriteLine($"{x.Text}");
                    }
                });
            while (true)
            {
            loop:

                Console.WriteLine("Please choose what to do !");
                Console.WriteLine("1.Send broadcast message");
                Console.WriteLine("2.Send direct message");
                Console.WriteLine("0.Exit !");
                int choosed;
                var choose = int.TryParse(Console.ReadLine(), out choosed);
                if (choose == false)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Something went wrong , please try again . . .");
                    Console.ResetColor();
                    goto loop;
                }
                switch (choosed)
                {
                    case 1:
                        // write
                        while (true)
                        {
                            var key = Console.ReadKey();

                            // A key input starts writing mode
                            lock (consoleLock)
                            {
                                var content = key.KeyChar + Console.ReadLine();
                                if (content == "0")
                                {
                                    break;
                                }
                                Client.SendMessage(new Message { Text = $"{content}",Sender =_userName });
                            }
                        }
                        Console.Clear();
                        break;
                    case 2:
                  

                        Console.WriteLine("enter username to send msg :");
                        string reciver = Console.ReadLine();
                        Console.WriteLine("enter msg:");
                        var key2 = Console.ReadKey();
                        var content2 = key2.KeyChar + Console.ReadLine();
                        Client.SendMessage(new Message { Text = $"{content2}" ,Receiver=reciver,Sender = _userName });
                        break;
                    default:
                        break;
                }

            }
        }
    }
}