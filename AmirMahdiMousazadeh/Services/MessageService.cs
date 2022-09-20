using AmirMahdiMousazadeh.Repositories;
using System.ComponentModel.Composition;
using System.Reactive.Linq;

namespace AmirMahdiMousazadeh.Services
{
   [Export]
    public class MessageService
    {
        

        [Import]
        private IMessageServiceRepository m_repository = null;
        private event Action<Message> Added;

        public void Add(Message message)
        {
            m_repository.Add(message);
            Added?.Invoke(message);
        }

        public IObservable<Message> GetChatLogsAsObservable(string reciver)
        {
            var oldLogs = m_repository.GetAll(reciver).ToObservable();
            var newLogs = Observable.FromEvent<Message>((x) => Added += x, (x) => Added -= x);
            var data = oldLogs.Concat(newLogs).Where(p => p.Receiver == reciver || p.Receiver == null || p.Receiver == "");
            return data;
        }
    }
}
