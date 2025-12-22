using Server.Serveices;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Запуск HTTP сервера...");
            HttpService.Start();

            await Task.Delay(-1);
        }
    }
}
