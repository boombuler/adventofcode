namespace AdventOfCode._2020;

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day20 : Solution
{
    class Tile
    {
        private List<Tile> fRotations;
        public long ID { get; }
        public int Size => Rows.Length;
        public string[] Rows { get; }
        public string Left { get; private set; }
        public string Right { get; private set; }
        public string Top => Rows.First();
        public string Bottom => Rows.Last();

        public Tile(long id, string[] rows)
            : this(id, rows, null)
        {
        }
        private Tile(long id, string[] rows, List<Tile> rotations)
        {
            (ID, Rows) = (id, rows);
            Left = new string(Rows.Select((r) => r[0]).ToArray());
            Right = new string(Rows.Select((r) => r[Size - 1]).ToArray());
            if (rotations != null)
            {
                fRotations = rotations;
                fRotations.Add(this);
            }
            else
                BuildRotations();
        }

        private Tile VFlip()
            => new(ID, Rows.Reverse().ToArray(), fRotations);

        private Tile Rotate()
        {
            var size = Size;
            var newRows = Enumerable.Range(0, size).Select(y =>
                new string(Enumerable.Range(0, size).Select(x => Rows[x][size - 1 - y]).ToArray())
            ).ToArray();
            return new Tile(ID, newRows, fRotations);
        }
        public IEnumerable<Tile> GetRotations() => fRotations;
        private void BuildRotations()
        {
            fRotations = new List<Tile> { this };
            Rotate().Rotate().Rotate().VFlip().Rotate().Rotate().Rotate();
        }

        public Tile WithoutBorder()
        {
            var len = Size - 2;
            return new Tile(ID, Rows.Skip(1).Take(len).Select(r => r.Substring(1, len)).ToArray());
        }
    }

    private static IEnumerable<Tile> ReadTiles(string input)
    {
        var data = input.Lines().Select((line, idx) => new { line, idx }).GroupBy(e => e.idx / 12, e => e.line);
        foreach (var g in data)
        {
            var titleLine = g.First();
            var id = long.Parse(titleLine[5..].TrimEnd(':'));
            var rows = g.Skip(1).TakeWhile(l => !string.IsNullOrEmpty(l)).ToArray();
            yield return new Tile(id, rows);
        }
    }

    private Tile[][] Solve(string input)
    {
        var items = ReadTiles(input).ToList();
        var size = (int)Math.Sqrt(items.Count);
        Tile[][] grid = Enumerable.Range(0, size).Select(_ => new Tile[size]).ToArray();

        bool FindSolutions(int idx)
        {
            int row = idx / size;
            int col = idx % size;
            var topEdge = row > 0 ? grid[row - 1][col].Bottom : null;
            var leftEdge = col > 0 ? grid[row][col - 1].Right : null;

            for (int i = 0; i < items.Count; i++)
            {
                var unassigned = items[i];
                items.RemoveAt(i);
                foreach (var rotated in unassigned.GetRotations())
                {
                    if (topEdge != null && (topEdge != rotated.Top))
                        continue;
                    if (leftEdge != null && (leftEdge != rotated.Left))
                        continue;

                    grid[row][col] = rotated;
                    if (items.Count == 0)
                        return true;

                    if (FindSolutions(idx + 1))
                        return true;
                }

                items.Insert(i, unassigned);
            }
            grid[row][col] = null;
            return false;
        }
        if (!FindSolutions(0))
        {
            Error("No solution -.-");
            return null;
        }
        return grid;
    }

    private long GetCornersValue(string inputFile)
    {
        var grid = Solve(inputFile);
        return grid.First().First().ID *
            grid.First().Last().ID *
            grid.Last().First().ID *
            grid.Last().Last().ID;
    }

    private long CountMonsterPixels(Tile image)
    {
        var pattern = new string[]
        {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   ",
        };
        int pRows = pattern.Length;
        int pCols = pattern[0].Length;

        long matches = 0;

        int size = image.Size;

        for (int imgRow = 0; imgRow < size - pRows; imgRow++)
        {
            for (int imgCol = 0; imgCol < size - pCols; imgCol++)
            {
                bool match = true;
                for (int patRow = 0; match && patRow < pRows; patRow++)
                {
                    for (int patCol = 0; match && patCol < pCols; patCol++)
                    {
                        if (pattern[patRow][patCol] == '#' && image.Rows[imgRow + patRow][imgCol + patCol] != '#')
                            match = false;
                    }
                }
                if (match)
                    matches++;
            }
        }
        return matches * pattern.Sum(r => r.Count(c => c == '#'));
    }

    private long GetRoughness(string inputFile)
    {
        // Solve input and merge to single tile.
        var result = Solve(inputFile).SelectMany(tileRow =>
        {
            var tiles = tileRow.Select(tr => tr.WithoutBorder()).ToList();
            return Enumerable.Range(0, tiles[0].Size)
                .Select(i => tiles.Select(t => t.Rows[i]).Aggregate(string.Concat));
        }).ToArray();
        var image = new Tile(0, result);

        var monsters = image.GetRotations().Select(CountMonsterPixels).OrderByDescending(mc => mc).FirstOrDefault();

        var waves = image.Rows.Sum(r => r.Count(c => c == '#'));

        return waves - monsters;
    }

    protected override long? Part1()
    {
        Assert(GetCornersValue(Sample()), 20899048083289);
        return GetCornersValue(Input);
    }

    protected override long? Part2()
    {
        Assert(GetRoughness(Sample()), 273);
        return GetRoughness(Input);
    }
}
