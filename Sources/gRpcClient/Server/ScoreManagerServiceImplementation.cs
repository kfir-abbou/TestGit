using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
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

        public override Task<ReplyMsg> StartNewMatch(StartMatchRequestMsg request, ServerCallContext context)
        {
            var reply = new ReplyMsg { ErrorMsg = "", Result = REPLY_MSG_RESULT.Succeed };

            var p1 = m_context.Players.Single(p => p.Id == request.Player1Id);
            var p2 = m_context.Players.Single(p => p.Id == request.Player2Id);

            var game = new Game();
            var set = new TennisSet
            {
                Games = new List<Game> { game }
            };

            m_context.Game.Add(game);
            m_context.TennisSet.Add(set);
            m_context.Match.Add(new Match
            {
                Sets = new List<TennisSet> { set },
                FirstPlayer = p1,
                SecondPlayer = p1
            });

            try
            {
                m_context.SaveData();
                var currentGameData = m_context.Game.OrderBy(g => g.Id).Last();
                var match = m_context.Match.OrderBy(g => g.Id).Last();

                reply.GameID = match.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return Task.FromResult(reply);
        }

        public override Task<ScoreBoardStatusReplyMsg> GetScoreBoardStatus(ScoreBoardStatusRequestMsg request, ServerCallContext context)
        {
            var match = m_context.Match.Single(m => m.Id == request.MatchId);
            if (match == null)
            {
                throw new ArgumentException($"Match with id: '{request.MatchId}' not found.");
            }

            //var game = m_context.Game;
            //var scores = m_context.GameScores;
            //var players = m_context.Players;
            
            foreach (var set in match.Sets)
            {
                if (set.Winner != null)
                {
                    // We have a winner no need to check games
                    // player winCount++
                    continue;
                }
                else // Check if a player won this Set
                {
                    if (set.Games.Count >= 6) // if score is 6-0
                    {
                        // var p1WinCount
                        // var p2WinCount

                        // if (p1 - p2 >= 2) -> p1 won
                        // if (p2 - p1 >= 2) -> p2 won
                    }
                    foreach (var game in set.Games)
                    {
                        if (game.Winner != null)
                        {
                            // Player winCount++  
                        }
                        else
                        {
                            // Count Scores
                            
                        }
                    }
                }
                
            }
            
            ////var scoresPerGame = scores.Where(s => s.GameId == request.GameID).OrderBy(s => s.Id).GroupBy(s => s.ScoringPlayerId);

            //var scoresPerGame = scores.Where(s => s.GameId == request.GameID)
            //    .OrderBy(s => s.Id).ToList().GroupBy(s => s.ScoringPlayerId);
            //var reply = new ScoreBoardStatusReplyMsg();

            //foreach (var s in scoresPerGame)
            //{
            //    // s is id -> so for each id we should count scores 

            //}

            //var statusList = new RepeatedField<gameStatus>();

            //foreach (var status in statusList)
            //{
            //    reply.GameStatus.Add(status);
            //}
            //return Task.FromResult(reply);
            return Task.FromResult(new ScoreBoardStatusReplyMsg());

        }

        public override Task<ReplyMsg> UpdateGameResult(UpdateGameRequestMsg request, ServerCallContext context)
        {
            // Update data
            Console.WriteLine($"Game -> {request.PlayerScored} Scored!");
            var game = m_context.Game.Single(g => request.GameID == g.Id);
            //   var player = m_context.Players.Single(p => p.Id == request.PlayerScored);

            m_context.GameScores.Add(new GameScores
            {
                Game = game,
                ScoringPlayer = (int)request.PlayerScored
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

//public override Task<ScoreReplyMsg> GetCurrentScore(GetGameDataRequestMsg request, ServerCallContext context)
//{
//    // Send db request for data 

//    var reply = new ScoreReplyMsg();
//    var scoresByGameId = m_context.GameScores.Where(gs => gs.GameId == request.GameID).OrderBy(gs => gs.Id).ToList();

//    if (scoresByGameId.Any())
//    {
//        var p1TotalScores = scoresByGameId.Where(gs => gs.ScoringPlayerId == (int)PLAYER._1).Select(gs => gs.Id);
//        var p2TotalScores = scoresByGameId.Where(gs => gs.ScoringPlayerId == (int)PLAYER._2).Select(gs => gs.Id);

//        var player1Scores = p1TotalScores.ToList();
//        var player2Scores = p2TotalScores.ToList();

//        reply.Player1Score = Utils.ScoreToStringConverter(player1Scores.Count());
//        reply.Player2Score = Utils.ScoreToStringConverter(player2Scores.Count());

//        var gameData = m_context.Game.Single(g => g.Id == request.GameID);
//        var p1 = m_context.Players.Single(p => p.Id == gameData.FirstPlayer);
//        var p2 = m_context.Players.Single(p => p.Id == gameData.SecondPlayer);
//        reply.P1Name = $"{p1.FirstName} {p1.LastName}";
//        reply.P2Name = $"{p2.FirstName} {p2.LastName}";

//        var status = checkScores(player1Scores.Count(), player2Scores.Count());
//        reply.GameStatus = status.ToString();
//        handleState(status);
//    }
//    return Task.FromResult(reply);
//}

//private void handleState(ScoreState state)
//{
//    switch (state)
//    {
//        case ScoreState.p1WonGame:
//            {
//                var gameData = m_context.Game.OrderBy(g => g.Id).Last();
//                gameData.Winner = gameData.FirstPlayer;
//                // now check if won set
//                // if so -> check if won match
//                break;
//            }
//        case ScoreState.p1WonSet:
//            break;
//        case ScoreState.p1WonMatch:
//            break;
//        case ScoreState.p2WonGame:
//            {
//                var gameData = m_context.Game.OrderBy(g => g.Id).Last();
//                gameData.Winner = gameData.SecondPlayer;
//                break;
//            }
//        case ScoreState.p2WonSet:
//            break;
//        case ScoreState.p2WonMatch:
//            break;
//        case ScoreState.tie:
//            break;
//        case ScoreState.still_playing:
//            break;
//        default:
//            throw new ArgumentOutOfRangeException(nameof(state), state, null);
//    }
//}

//private ScoreState checkScores(int p1Scores, int p2Scores)
//{
//    if (p1Scores > p2Scores)
//    {
//        if (p1Scores > 3 && p1Scores - p2Scores == 2)
//        {
//            // p1 won
//            return ScoreState.p1WonGame;
//        }
//    }
//    else if (p2Scores > p1Scores)
//    {
//        if (p2Scores > 3 && p2Scores - p1Scores == 2)
//        {
//            // p2 won
//            return ScoreState.p2WonGame;
//        }
//    }
//    else // Tie
//    {
//        if (p1Scores == 3) // 40
//        {

//        }
//    }

//    return ScoreState.still_playing;
//}
