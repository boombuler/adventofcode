namespace AdventOfCode._2021;

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day21 : Solution
{
    record GameState(Player Player1, Player Player2)
    {
        public IEnumerable<(GameState State, long Count)> StepQuantumGames(int activePlayer)
        {
            var amounts = new[] { -1, -1, -1, 1, 3, 6, 7, 6, 3, 1 };
            for (int dRes = 3; dRes <= 9; dRes++)
            {
                var count = amounts[dRes];
                if (activePlayer == 0)
                    yield return (this with { Player1 = Player1.Step(dRes) }, count);
                else
                    yield return (this with { Player2 = Player2.Step(dRes) }, count);
            }
        }
        public bool PlayerWon(int player, int score) => ((player == 0) ? Player1 : Player2).Won(score);
    }
    record Player(int Position, int Score)
    {
        public bool Won(int minScore) => Score >= minScore;
        public Player Step(int amount)
        {
            var pos = (Position + amount) % 10;
            return new Player(pos, Score + pos + 1);
        }
    }
    private static (Player, Player) GetPlayers(string input)
    {
        var (p1, (p2, _)) = input.Lines().Select(line => line.Split(' ').Last()).Select(int.Parse);
        return (new Player(p1 - 1, 0), new Player(p2 - 1, 0));
    }

    private static long PlayDeterministicGame(string startPositions)
    {
        var (a, b) = GetPlayers(startPositions);
        long rollCount = 0;
        var die = (-1).Unfold(i => (i + 1) % 100).Select(i => i + 1).Chunk(3).Select(c => c.Sum());
        foreach (var dval in die)
        {
            rollCount += 3;
            a = a.Step(dval);
            if (a.Won(1000))
                return b.Score * rollCount;
            (a, b) = (b, a);
        }
        return 0;
    }

    private static long PlayQuantumGame(string startPositions)
    {
        var (a, b) = GetPlayers(startPositions);
        var openGames = new Dictionary<GameState, long>() { { new GameState(a, b), 1 } };
        var wins = new long[2];
        var activePlayer = 0;

        while (openGames.Count > 0)
        {
            var next = from game in openGames
                       from newGame in game.Key.StepQuantumGames(activePlayer)
                       group game.Value * newGame.Count by newGame.State into games
                       select new { Game = games.Key, Count = games.Sum() };
            openGames = new Dictionary<GameState, long>();
            foreach (var g in next)
            {
                if (g.Game.PlayerWon(activePlayer, 21))
                    wins[activePlayer] += g.Count;
                else
                    openGames[g.Game] = g.Count;
            }
            activePlayer ^= 1;
        }

        return wins.Max();
    }

    protected override long? Part1()
    {
        Assert(PlayDeterministicGame(Sample()), 739785);
        return PlayDeterministicGame(Input);
    }

    protected override long? Part2()
    {
        Assert(PlayQuantumGame(Sample()), 444356092776315);
        return PlayQuantumGame(Input);
    }
}
