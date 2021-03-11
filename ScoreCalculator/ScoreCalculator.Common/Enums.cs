namespace ScoreCalculator.EF
{
    public enum Score
    {
        Love = 0,
        Fifteen = 1,
        Thirty = 2,
        Forty = 3,
        one = 4,
        two = 5
    }

    public enum ScoreState
    {
        p1WonGame = 0,
        p1WonSet,
        p1WonMatch,
        p2WonGame,
        p2WonSet,
        p2WonMatch,
        tie,
        still_playing
    }
}
