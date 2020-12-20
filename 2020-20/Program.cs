using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2020_20
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        enum Edge { Top, Bottom, Left, Right}

        class Tile
        {
            public string[] Rows { get; }
            private string fLeft;
            private string fRight;
            private List<Tile> fRotations;
            public long ID { get; }

            public Tile(long id, string[] rows)
                : this(id, rows, null)
            {
            }
            private Tile(long id, string[] rows, List<Tile> rotations)
            {
                (ID, Rows) = (id, rows);
                Rows.Select((r) => r[0]).ToArray();
                fLeft = new string(Rows.Select((r) => r[0]).ToArray());
                fRight = new string(Rows.Select((r) => r[9]).ToArray());
                if (rotations != null)
                {
                    fRotations = rotations;
                    fRotations.Add(this);
                }
                else
                    BuildRotations();
            }

            private Tile VFlip() => new Tile(ID, Rows.Reverse().ToArray(), fRotations);

            private Tile Rotate()
            {
                var w = Rows[0].Length;
                var newRows = Enumerable.Range(0, w).Select(y =>
                    new string(Enumerable.Range(0, w).Select(x => Rows[x][w - 1 - y]).ToArray())
                ).ToArray();
                return new Tile(ID, newRows, fRotations);
            }
            public IEnumerable<Tile> GetRotations() => fRotations;
            private void BuildRotations()
            {
                fRotations = new List<Tile>();
                fRotations.Add(this);
                Rotate().Rotate().Rotate().VFlip().Rotate().Rotate().Rotate();
            }
            public override string ToString() => ID.ToString();
            public string GetEdge(Edge e)
            {
                switch(e)
                {
                    case Edge.Top: return Rows[0];
                    case Edge.Bottom: return Rows[9];
                    case Edge.Left: return fLeft;
                    case Edge.Right: return fRight;
                }
                throw new InvalidOperationException();
            }
        }

        private IEnumerable<Tile> ReadTiles(string input)
        {
            var data = ReadLines(input).Select((line, idx) => new { line, idx }).GroupBy(e => e.idx / 12);
            foreach(var g in data)
            {
                var titleLine = g.First().line;
                var id = long.Parse(titleLine.Substring(5).TrimEnd(':'));
                var rows = g.Skip(1).Take(10).Select(e => e.line).ToArray();
                yield return new Tile(id, rows);
            }
        }

        private Tile[,] Solve(string inputFile)
        {
            var items = ReadTiles(inputFile).ToList();
            var size = (int)Math.Sqrt(items.Count);
            Tile[,] grid = new Tile[size, size];

            bool FindSolutions(int idx)
            {
                int row = idx / size;
                int col = idx % size;
                var topEdge = row > 0 ? grid[row - 1, col].GetEdge(Edge.Bottom) : null;
                var leftEdge = col > 0 ? grid[row, col - 1].GetEdge(Edge.Right) : null;

                for (int i = 0; i < items.Count; i++)
                {
                    var ua = items[i];
                    items.RemoveAt(i);
                    foreach (var rot in ua.GetRotations())
                    {
                        if (topEdge != null && (topEdge != rot.GetEdge(Edge.Top)))
                            continue;
                        if (leftEdge != null && (leftEdge != rot.GetEdge(Edge.Left)))
                            continue;

                        grid[row, col] = rot;
                        if (items.Count == 0)
                            return true;

                        if (FindSolutions(idx + 1))
                            return true;
                    }

                    items.Insert(i, ua);
                }
                grid[row, col] = null;
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
            var size = grid.GetLength(0);
            return grid[0, 0].ID * grid[0, size - 1].ID * grid[size - 1, 0].ID * grid[size - 1, size - 1].ID;
        }

        private long CountMonsters(Tile image)
        {
            var pattern = new string[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   ",
            };

            long monsters = 0;

            for (int line = 0; line < image.Rows.Length - (pattern.Length - 1); line++)
            {
                for (int i = 0; i < image.Rows[line].Length - (pattern[0].Length - 1); i++)
                {
                    bool match = true;
                    for (int ml = 0; ml < pattern.Length; ml++)
                    {
                        for (int mi = 0; match && mi < pattern[ml].Length; mi++)
                        {
                            if (pattern[ml][mi] == '#' && image.Rows[line + ml][i + mi] != '#')
                            {
                                match = false;
                            }
                        }
                    }
                    if (match)
                        monsters++;
                }
            }
            return monsters;
        }

        private long GetRoughness(string inputFile)
        {
            var grid = Solve(inputFile);
            
            var merged = Enumerable.Range(0, grid.GetLength(0) * 10)
                .Where(r =>
                {
                    var rowIdx = r % 10;
                    return rowIdx != 0 && rowIdx != 9;
                })
                .Select(r => string.Join(string.Empty, Enumerable.Range(0, grid.GetLength(0))
                        .Select(c => grid[r / 10, c])
                        .Select(t => t.Rows[r % 10].Substring(1, 8)))
                ).ToArray();

            var image = new Tile(0, merged);

            var monsters = image.GetRotations().Select(CountMonsters).OrderByDescending(mc => mc).FirstOrDefault();

            var waves = image.Rows.Select(r => r.Where(c => c == '#').Count()).Sum();

            return waves - (15 * monsters);
        }

        protected override long? Part1()
        {
            Assert(GetCornersValue("Sample.txt"), 20899048083289);
            return GetCornersValue("Input.txt");
        }

        protected override long? Part2()
        {
            Assert(GetRoughness("Sample.txt"), 273);
            return GetRoughness("Input.txt");
        }
    }
}
