using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScoreCalculator.EF;


namespace ScoreManagerService.Server
{
    public class ScoreManagerServer
    {
        private readonly Grpc.Core.Server m_server;

        public ScoreManagerServer()
        {
            m_server = new Grpc.Core.Server()
            {
                Services = { Proto.ScoreManagerService.BindService(new ScoreManagerServiceImplementation(new ScoreCalcContext(new DbContextOptions<ScoreCalcContext>()))) },
                Ports = { new ServerPort("localhost", 11111, ServerCredentials.Insecure) }
            };
        }

        public void Start()
        {
            Console.WriteLine($"Server started on ->  {m_server.Ports}");
            m_server.Start();
        }

        public async Task ShutdownAsync()
        {
            await m_server.ShutdownAsync();
        }
    }
}
