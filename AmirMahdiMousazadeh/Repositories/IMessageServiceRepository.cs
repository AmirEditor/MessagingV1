namespace AmirMahdiMousazadeh.Repositories
{
    public interface IMessageServiceRepository
    {
        void Add(Message message);
        IEnumerable<Message> GetAll(string reciver);
    }
}
