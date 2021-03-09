using System;
using ScoreManagerService.Server;

namespace ScoreManagerService
{
    class Program
    {

        static void Main()
        {
            var server = new ScoreManagerServer();
            server.Start();
            
            Console.WriteLine("Press any key to stop server...");

            Console.ReadKey();
        }
    }
}
