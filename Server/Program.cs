namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {                    
            
            var server = new Server("127.0.0.1", 2024);
            await server.StartAsync();
        }
    }
}
