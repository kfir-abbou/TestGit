using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Grpc.Core;
using Proto;
using ScoreCalculator.EF;

namespace GameScoreEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Channel m_channel;
        private ScoreManagerService.ScoreManagerServiceClient m_client;
        private int m_currentGameId;
        private readonly IList<string> m_playersNames = new List<string>();
        private readonly IList<TennisPlayer> m_players = new List<TennisPlayer>();

        public MainWindow()
        {
            InitializeComponent();
            if (Debugger.IsAttached)
            {
                if (Process.GetProcesses().All(p => p.ProcessName != "ScoreManagerService"))
                {
                    Process.Start(@"C:\GIT\TestGit\Sources\gRpcClient\bin\Debug\netcoreapp3.1\ScoreManagerService.exe");
                }
            }
           
            Thread.Sleep(1000);
            refreshPlayersList();
            FirstPlayerCb.SelectedIndex = 0;
            SecondPlayerCb.SelectedIndex = 0;

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            m_channel = new Channel("localhost", 11111, ChannelCredentials.Insecure);
            m_client = new ScoreManagerService.ScoreManagerServiceClient(m_channel);
        }
        
        private async void FirstScoreBtn_Click(object sender, RoutedEventArgs e)
        {
            var req = new UpdateGameRequestMsg
            {
                GameID = m_currentGameId,
                PlayerScored = PLAYER._1
            };
            var rep = await m_client.UpdateGameResultAsync(req);
            LogRichText($"First Player Scored -> {rep.Result}");
            //GetCurrentScoreBtn_Click(null, null);

            //var statusReply = await m_client.GetScoreBoardStatusAsync(new GetGameDataRequestMsg
            //{
            //    GameID = m_currentGameId
            //});
        }

        private async void SecondScoreBtn_Click(object sender, RoutedEventArgs e)
        {
            var req = new UpdateGameRequestMsg
            {
                GameID = m_currentGameId,
                PlayerScored = PLAYER._2
            };
            var rep = await m_client.UpdateGameResultAsync(req);
            LogRichText($"Second Player Scored -> {rep.Result}");
            //GetCurrentScoreBtn_Click(null, null);
        }

        private void GetCurrentScoreBoardDataBtn_Click(object sender, RoutedEventArgs e)
        {
            var rep = m_client.GetScoreBoardStatus(new ScoreBoardStatusRequestMsg
            {
                MatchId = m_currentGameId
            });
            
            //LogRichText($"Score Board -> {rep.P1Name}: {rep.Player1Score}\n{rep.P2Name}: {rep.Player2Score}, Status: {rep.GameStatus}");
        }

        private void LogRichText(string p_data)
        {
            RichTb.AppendText(p_data);
            RichTb.AppendText("\n");
        }

        private void AddPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            var reply = m_client.AddPlayer(new AddPlayerRequestMsg
            {
                First = FirstNameTb.Text,
                Last = LastNameTb.Text
            });
            if (reply.Result == REPLY_MSG_RESULT.Succeed)
            {
                LogRichText($"Player: {FirstNameTb.Text} {LastNameTb.Text} was added.");
            }
            else
            {
                LogRichText($"Error: {reply.ErrorMsg}");
            }
            refreshPlayersList();
        }

        private void refreshPlayersList()
        {
            var reply = m_client.GetPlayers(new GetPlayerRequestMsg());
            var players = reply.PlayersData;
            if (players.Count == m_players.Count) return;
            
            foreach (var plyr in players)
            {
                if (m_playersNames.Contains($"{plyr.First} {plyr.Last}"))
                {
                    continue;
                }
                m_playersNames.Add($"{plyr.First} {plyr.Last}");
                m_players.Add(new TennisPlayer
                {
                    Id = plyr.Id,
                    FirstName = plyr.First,
                    LastName = plyr.Last
                });
            }

            FirstPlayerCb.ItemsSource =new List<string>(m_playersNames);
            SecondPlayerCb.ItemsSource = new List<string>(m_playersNames);
        }

      
        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            var firstSplited = FirstPlayerCb.SelectedValue.ToString()?.Split(" ");
            var secondSpilted = SecondPlayerCb.SelectedValue.ToString()?.Split(" ");

            var p1 = m_players.SingleOrDefault(p => p.FirstName == firstSplited.First() && p.LastName == firstSplited.Last()).Id;
            var p2 = m_players.SingleOrDefault(p => p.FirstName == secondSpilted.First() && p.LastName == secondSpilted.Last()).Id;
            
            var initData = new StartMatchRequestMsg
            {
                Player1Id = p1,
                Player2Id = p2
            };

            var rep = m_client.StartNewMatch(initData);
            if (rep.Result == REPLY_MSG_RESULT.Succeed)
            {
                m_currentGameId = rep.GameID;
            }
            Console.WriteLine($"[InitBtn_Click] reply: {rep}");
            LogRichText($"Game with id: {rep.GameID} Started... ");
            //return rep;
        }
    }
}
