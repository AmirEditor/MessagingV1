using AmirMahdiMousazadeh.Extentions;
using AmirMahdiMousazadeh.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.ComponentModel.Composition;
using System.Reactive.Linq;

namespace AmirMahdiMousazadeh
{
    public class MessengerService : Messenger.MessengerBase
    {
        [Import]
        private MessageService message = MEFManager.Container.GetExportedValues<MessageService>().FirstOrDefault();


        private readonly ILogger<MessengerService> _logger;

        public MessengerService(
            ILogger<MessengerService> logger)
        {

            _logger = logger;

        }
        public override async Task<Empty> BroadcastMessage(Empty request, ServerCallContext context)
        {

            return await Task.FromResult(new Empty());
        }
        public override async Task<Empty> SendMessage(Message request, ServerCallContext context)
        {
            message.Add(request);
            return await Task.FromResult(new Empty());
        }
        public override async Task SubscribeMessages(UserInfo request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} subscribes.");
            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels subscription."));

            try
            {
                    await message.GetChatLogsAsObservable(request.Name)
                   .ToAsyncEnumerable()
                   .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(new Message { Text = $"{x.Sender} says : {x.Text}", Receiver = x.Receiver }), context.CancellationToken)
                   .ConfigureAwait(false);
                
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"{peer} unsubscribed.");
            }
        }
    }
}
