using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Proto;
using ScoreCalculator.EF;


/*
 * SERVER
 */
namespace ScoreDataClient
{
    class Program
    {
        private static Channel m_channel;
        private static ScoreManagerService.ScoreManagerServiceClient m_client;

        //static void Main(string[] args)
        //{
        //    using var ctx = new ScoreCalcContext(new DbContextOptions<ScoreCalcContext>());
        //    var game = new Game
        //    {
        //        FirstPlayerName = "Kfir",
        //        SecondPlayerName = "Dida",
        //    };

        //    ctx.Game.Add(game);
        //    ctx.SaveChanges();

        //    Console.WriteLine("Great!");
        //}

        static void Main() //async
        {
            //    CreateChannelAndClient();

            //    var init = await InitScoreServiceAsync();

            //    Console.WriteLine($"Init done -> {init}");

            //    await Task.Delay(TimeSpan.FromSeconds(1));

            //    var scoreSet = await SetScore();

            //    Console.WriteLine($"Score Set -> {scoreSet}");

            //    await Task.Delay(TimeSpan.FromSeconds(1));

            //    var scoreData = await GetCurrentScore();

            //    await Task.Delay(TimeSpan.FromSeconds(1));

            //    Console.WriteLine($"Score Data -> Player 1: {scoreData.Player1Score}, Player 2: {scoreData.Player2Score}");
            //    Console.ReadKey();
        }

        //    static void CreateChannelAndClient()
        //    {
        //        m_channel = new Channel("localhost", 11111, ChannelCredentials.Insecure);
        //        m_client = new ScoreManagerService.ScoreManagerServiceClient(m_channel);
        //        Console.WriteLine("Channel and client Created.");
        //    }

        //    static async Task<ReplyMsg> InitScoreServiceAsync()
        //    {
        //        var initData = new InitRequestMsg
        //        {
        //            Player1Name = "Kfir Abbou",
        //            Player2Name = "Dida Abbou"
        //        };

        //        var rep = await m_client.InitServiceAsync(initData);
        //        return rep;
        //    }

        //    static async Task<ReplyMsg> SetScore()
        //    {
        //        var scoreData = new UpdateGameRequestMsg
        //        {
        //            PlayerScored = PLAYER._1
        //        };
        //        var rep = await m_client.UpdateGameResultAsync(scoreData);
        //        return rep;
        //    }

        //    static async Task<ScoreReplyMsg> GetCurrentScore()
        //    {
        //        var rep = await m_client.GetCurrentScoreAsync(new GetCurrentScoreRequestMsg());
        //        return rep;
        //    }
    }
}
