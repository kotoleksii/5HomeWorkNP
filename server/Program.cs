using System;
using System.Threading;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerEntity server = null; 
            
            try
            {
                server = new ServerEntity();
                new Thread(server.Listen).Start();
            }
            catch (Exception ex)
            {
                server?.DisconnectAll();
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}
