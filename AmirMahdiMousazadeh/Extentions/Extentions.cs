namespace AmirMahdiMousazadeh.Extentions
{
    public static class Extentions
    {
        public static async Task ForEachAsync(this List<Message> list, Func<Message, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }
    }
}
