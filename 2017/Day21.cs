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
        private const string StartingImage = ".#./..#/###";
        private const char LineSep = '/';
        private IEnumerable<string> WalkRegions(string image)
        {
            var lines = image.Split(LineSep);
            int size = lines.Length % 2 == 0 ? 2 : 3;
            
            for (int y = 0; y < lines.Length; y+=size)
            { 
                for (int x = 0; x < lines.Length; x+=size)
                    yield return string.Join(LineSep, Enumerable.Range(y, size).Select(l => lines[l].Substring(x, size)));
            }
        }
       
        private string Rotate(string s)
        {
            var rows = s.Split(LineSep);
            var size = rows.Length;
            return string.Join(LineSep, Enumerable.Range(0, size).Select(y =>
                new string(Enumerable.Range(0, size).Select(x => rows[x][size - 1 - y]).ToArray())
            ));
        }

        private string CombineRegions(IEnumerable<string> regions)
        {
            var rgns = regions.Select(r => r.Split(LineSep)).ToList();
            var size = (int)Math.Sqrt(rgns.Count);
            var sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int row = 0; row < rgns[i].Length; row++)
                {
                    if (sb.Length > 0)
                        sb.Append(LineSep);
                    for (int r = 0; r < size; r++)
                        sb.Append(rgns[(i * size) + r][row]);    
                }
            }
            return sb.ToString();
        }

        private long Enhance(string rules, int rounds)
        {
            var img = StartingImage;
            Dictionary<string, string> patterns = new Dictionary<string, string>();
            foreach(var rule in rules.Lines().Select(l => l.Split(" => ")))
            {
                var inp = rule[0];
                var flipped = string.Join(LineSep, inp.Split(LineSep).Reverse());
                patterns[flipped] = patterns[inp] = rule[1];

                for (int i = 0; i < 3; i++)
                {
                    (inp, flipped) = (Rotate(inp), Rotate(flipped));
                    patterns[flipped] = patterns[inp] = rule[1];
                }
            }

            for (int i = 0; i < rounds; i++)
                img = CombineRegions(WalkRegions(img).Select(r => patterns[r]));

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
