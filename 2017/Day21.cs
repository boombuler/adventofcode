using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2017
{
    class Day21 : Solution
    {
        class Pattern
        {
            public string Result { get; }
            private readonly HashSet<string> Inputs = new HashSet<string>();
            public Pattern(string rule)
            {
                var parts = rule.Split(" => ");
                Result = parts[1];
                
                var inp = parts[0];
                var flipped = string.Join('/', inp.Split('/').Reverse());
                Inputs.Add(inp);
                Inputs.Add(flipped);

                for (int i = 0; i < 3; i++)
                {
                    (inp, flipped) = (Rotate(inp), Rotate(flipped));
                    Inputs.Add(inp);
                    Inputs.Add(flipped);
                }
            }

            private string Rotate(string s)
            {
                var rows = s.Split('/');
                var size = rows.Length;
                return string.Join('/', Enumerable.Range(0, size).Select(y =>
                    new string(Enumerable.Range(0, size).Select(x => rows[x][size - 1 - y]).ToArray())
                ));
            }

            public bool Matches(string region)
                => Inputs.Contains(region);
        }

        private const string StartingImage = ".#./..#/###";

        private IEnumerable<string> WalkRegions(string image)
        {
            var lines = image.Split('/');
            if (lines.Length % 2 == 0)
            {
                for (int y = 0; y < lines.Length; y+=2)
                { 
                    for (int x = 0; x < lines.Length; x+=2)
                        yield return new string(new char[]
                        {
                            lines[y+0][x], lines[y+0][x+1], '/', 
                            lines[y+1][x], lines[y+1][x+1]
                        });
                }
            }
            else
            {
                for (int y = 0; y < lines.Length; y += 3)
                {
                    for (int x = 0; x < lines.Length; x += 3)
                        yield return new string(new char[]
                        {
                            lines[y+0][x], lines[y+0][x+1],lines[y+0][x+2], '/',
                            lines[y+1][x], lines[y+1][x+1],lines[y+1][x+2], '/',
                            lines[y+2][x], lines[y+2][x+1],lines[y+2][x+2],
                        });
                }
            }
        }

        public string CombineRegions(IEnumerable<string> regions)
        {
            var rgns = regions.Select(r => r.Split('/')).ToList();
            var size = (int)Math.Sqrt(rgns.Count);
            var sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int row = 0; row < rgns[i].Length; row++)
                {
                    if (sb.Length > 0)
                        sb.Append('/');
                    for (int r = 0; r < size; r++)
                        sb.Append(rgns[(i * size) + r][row]);
                    
                }
            }
            return sb.ToString();
        }

        private long Enhance(string rules, int rounds)
        {
            var img = StartingImage;
            var patterns = rules.Lines().Select(r => new Pattern(r)).ToList();

            string enhanceRegion(string region) => patterns.First(p => p.Matches(region)).Result;
            
            for (int i = 0; i < rounds; i++)
                img = CombineRegions(WalkRegions(img).Select(enhanceRegion));

            return img.Where(c => c == '#').LongCount();
        }

        protected override long? Part1()
        {
            Assert(Enhance(Sample(), 2), 12);
            return Enhance(Input, 5);
        }

        protected override long? Part2() => Enhance(Input, 18);
    }
}
