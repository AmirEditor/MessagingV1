using ServiceStack.Redis;
using System.ComponentModel.Composition;

namespace AmirMahdiMousazadeh.Repositories
{
    [Export(typeof(IMessageServiceRepository))]
    public class MessageServiceRepository : IMessageServiceRepository
    {
        public void Add(Message message)
        {
            using (var connection = new RedisClient())
            {
                var messages = connection.Get<List<Message>>("Messages");
                if (messages == null)
                {
                    messages = new List<Message>();
                }

                    messages.Add(message);
                    connection.Remove("Messages");
                    connection.Add("Messages", messages);
            }
        }

        public IEnumerable<Message> GetAll(string reciver)
        {
            using (var connection = new RedisClient())
            {
                var messages = connection.Get<List<Message>>("Messages");
                if (messages == null)
                {
                    messages = new List<Message>();
                }
                return messages.Where(p=> p.Receiver == reciver || reciver ==null || reciver =="");
            }
        }
    }
}
