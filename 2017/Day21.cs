﻿namespace AdventOfCode._2017;

class Day21 : Solution
{
    private const string STARTING_IMAGE = ".#./..#/###";
    private const char LINE_SEP = '/';
    private static IEnumerable<string> WalkRegions(string image)
    {
        var lines = image.Split(LINE_SEP);
        int size = lines.Length % 2 == 0 ? 2 : 3;

        for (int y = 0; y < lines.Length; y += size)
        {
            for (int x = 0; x < lines.Length; x += size)
                yield return string.Join(LINE_SEP, Enumerable.Range(y, size).Select(l => lines[l].Substring(x, size)));
        }
    }

    private static string Rotate(string s)
    {
        var rows = s.Split(LINE_SEP);
        var size = rows.Length;
        return string.Join(LINE_SEP, Enumerable.Range(0, size).Select(y =>
            new string(Enumerable.Range(0, size).Select(x => rows[x][size - 1 - y]).ToArray())
        ));
    }

    private static string CombineRegions(IEnumerable<string> regions)
    {
        var rgns = regions.Select(r => r.Split(LINE_SEP)).ToList();
        var size = (int)Math.Sqrt(rgns.Count);
        var sb = new StringBuilder();
        for (int i = 0; i < size; i++)
        {
            for (int row = 0; row < rgns[i].Length; row++)
            {
                if (sb.Length > 0)
                    sb.Append(LINE_SEP);
                for (int r = 0; r < size; r++)
                    sb.Append(rgns[(i * size) + r][row]);
            }
        }
        return sb.ToString();
    }

    private static long Enhance(string rules, int rounds)
    {
        var img = STARTING_IMAGE;
        var patterns = new Dictionary<string, string>();
        foreach (var rule in rules.Lines().Select(l => l.Split(" => ")))
        {
            var inp = rule[0];
            var flipped = string.Join(LINE_SEP, inp.Split(LINE_SEP).Reverse());
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
