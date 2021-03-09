using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using Grpc.Core;
using Proto;
using ScoreCalculator.EF;

namespace ScoreManagerService.Server
{
    public class ScoreManagerServiceImplementation : Proto.ScoreManagerService.ScoreManagerServiceBase
    {
        private readonly IScoreCalcContext m_context;

        public ScoreManagerServiceImplementation(IScoreCalcContext p_context)
        {
            m_context = p_context;
        }
        // TODO: should be rename to "StartNewMatch"
        public override Task<ReplyMsg> InitService(InitRequestMsg request, ServerCallContext context)
        {
            var reply = new ReplyMsg { ErrorMsg = "", Result = REPLY_MSG_RESULT.Succeed };
            //if (string.IsNullOrEmpty(request.) || string.IsNullOrEmpty(request.Player2Name))
            //{
            //    reply = new ReplyMsg { ErrorMsg = "Players data not valid", Result = REPLY_MSG_RESULT.FailedOnError };
            //    return Task.FromResult(reply);
            //}

            //var firstPlayer = new TennisPlayer
            //{
            //    Id = request.Player1Id,
            //    FirstName = m_context.Players.Single(p => p.Id == request.Player1Id).FirstName,
            //    LastName = m_context.Players.Single(p => p.Id == request.Player1Id).LastName
            //};
            //var secondPlayer = new TennisPlayer
            //{
            //    FirstName = m_context.Players.Single(p => p.Id == request.Player2Id).FirstName,
            //    LastName = m_context.Players.Single(p => p.Id == request.Player2Id).LastName
            //};
            //m_context.Players.Add(firstPlayer);
            //m_context.Players.Add(secondPlayer);

            //var firstPlayerId = m_context.Players
            //    .First(p => p.FirstName == firstPlayer.FirstName && p.LastName == firstPlayer.LastName).Id;

            //var secondPlayerId = m_context.Players
            //    .First(p => p.FirstName == secondPlayer.FirstName && p.LastName == secondPlayer.LastName).Id;
            m_context.Game.Add(new Game
            {
                FirstPlayer = request.Player1Id,
                SecondPlayer = request.Player2Id,
            });

            m_context.SaveData();
            var currentGameData = m_context.Game
                .SingleOrDefault(g => g.FirstPlayer == request.Player1Id && g.SecondPlayer == request.Player2Id);
            if (currentGameData != null)
            {
                reply.GameID = currentGameData.Id;
            }

            return Task.FromResult(reply);
        }

        public override Task<ScoreReplyMsg> GetCurrentScore(GetCurrentScoreRequestMsg request, ServerCallContext context)
        {
            // Send db request for data 

            var reply = new ScoreReplyMsg();
            var score = m_context.GameScores.Where(gs => gs.GameId == request.GameID).OrderBy(gs => gs.Id).ToList();

            if (score.Any())
            {

                var totalPlayer1Scores = score.Where(gs => gs.ScoringPlayerId == (int) PLAYER._1).Select(gs => gs.Id);
                var totalPlayer2Scores = score.Where(gs => gs.ScoringPlayerId == (int) PLAYER._2).Select(gs => gs.Id);
                reply.Player1Score = Utils.ScoreToStringConverter(totalPlayer1Scores.Count());
                reply.Player2Score = Utils.ScoreToStringConverter(totalPlayer2Scores.Count());
            }
            return Task.FromResult(reply);

        }

        public override Task<ReplyMsg> UpdateGameResult(UpdateGameRequestMsg request, ServerCallContext context)
        {
            // Update data
            Console.WriteLine($"Game -> {request.PlayerScored} Scored!");

            m_context.GameScores.Add(new GameScores
            {
                GameId = request.GameID,
                ScoringPlayerId = (int)request.PlayerScored
            });
            try
            {
                m_context.SaveData();

                var reply = new ReplyMsg { Result = REPLY_MSG_RESULT.Succeed };
                return Task.FromResult(reply);
            }
            catch (Exception e)
            {
                return Task.FromResult(new ReplyMsg
                {
                    Result = REPLY_MSG_RESULT.FailedOnError,
                    ErrorMsg = $"{e}"
                });
            }
        }

        public override Task<ReplyMsg> AddPlayer(AddPlayerRequestMsg request, ServerCallContext context)
        {
            var reply = new ReplyMsg();
            if (!m_context.Players.Any
                (p => p.FirstName == request.First && p.LastName == request.Last))
            {
                m_context.Players.Add(new TennisPlayer
                {
                    FirstName = request.First,
                    LastName = request.Last
                });
                m_context.SaveData();
                reply.Result = REPLY_MSG_RESULT.Succeed;
            }
            else
            {
                reply.Result = REPLY_MSG_RESULT.FailedOnError;
                reply.ErrorMsg = "Player already exist";
            }

            return Task.FromResult(reply);
        }

        public override Task<GetPlayersReplyMsg> GetPlayers(GetPlayerRequestMsg request, ServerCallContext context)
        {
            var players = new List<TennisPlayer>(m_context.Players);

            var reply = new GetPlayersReplyMsg
            {
                Reply = new ReplyMsg
                {
                    Result = REPLY_MSG_RESULT.Succeed
                }
            };

            foreach (var tennisPlayer in players)
            {
                reply.PlayersData.Add(new AddPlayerRequestMsg
                {
                    Id = tennisPlayer.Id,
                   First = tennisPlayer.FirstName,
                    Last = tennisPlayer.LastName
                });
            }

            return Task.FromResult(reply);
        }
    }




}
