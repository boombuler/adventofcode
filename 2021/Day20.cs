namespace AdventOfCode._2021;

using System.Collections;

class Day20 : Solution
{
    public static long Enhance(string input, int rounds)
    {
        var lookup = new BitArray(input.Lines().First().Select(c => c == '#').ToArray());
        var img = new BitArray(input.Lines().Skip(2).SelectMany(line => line.Select(c => c == '#')).ToArray());
        int size = input.Lines().Count() - 2;
        bool everythingLit = false;

        for (int i = 0; i < rounds; i++)
        {
            var nextSize = size + 2;
            var next = new BitArray((int)(nextSize * nextSize));

            foreach (var px in Point2D.Range((-1, -1), (size, size)))
            {
                var window = from y in Enumerable.Range((int)px.Y - 1, 3)
                             from x in Enumerable.Range((int)px.X - 1, 3)
                             select new Point2D(x, y);

                var lookupIdx = window.Select((p, i) =>
                {
                    if (p.X >= 0 && p.Y >= 0 && p.X < size && p.Y < size)
                        return img[(int)(p.X + (size * p.Y))];
                    return everythingLit;
                }).Select((p, i) => (p ? 1 : 0) << (8 - i)).Aggregate((a, b) => a | b);

                if (lookup[lookupIdx])
                {
                    var idx = (int)((px.X + 1) + ((px.Y + 1) * nextSize));
                    next[idx] = true;
                }
            }
            everythingLit ^= lookup[0];
            img = next;
            size = nextSize;
        }
        return everythingLit ? long.MaxValue : img.OfType<bool>().Count(x => x);
    }

    protected override long? Part1()
    {
        Assert(Enhance(Sample(), 2), 35);
        return Enhance(Input, 2);
    }

    protected override long? Part2()
    {
        Assert(Enhance(Sample(), 50), 3351);
        return Enhance(Input, 50);
    }
}
