namespace AdventOfCode._2021;

class Day08 : Solution
{
    const string ALPHABET = "abcdefg";

    private static int SegmentBitsFromStr(string s) => s.Select(c => (1 << ALPHABET.IndexOf(c))).Aggregate((a, b) => a | b);

    private long GetOutput(string line)
    {
        var (notes, (output, _)) = line.Split(" | ");
        var samples = notes.Split(' ').Select(n => new { n.Length, Bits = SegmentBitsFromStr(n) }).Distinct().ToLookup(n => n.Length, n => n.Bits);

        var one = samples[2].Single();
        var three = samples[5].Single(s => (s & one) == one);
        var four = samples[4].Single();
        var six = samples[6].Single(s => (s & one) != one);
        var seven = samples[3].Single();
        var eight = samples[7].Single();
        var zero = samples[6].Single(s => (s & six & four) != (six & four));
        var nine = samples[6].Single(s => s != six && s != zero);
        var five = samples[5].Single(s => (s | one) == nine);
        var two = samples[5].Single(s => s != five && s != three);
        var lookup = new List<int> { zero, one, two, three, four, five, six, seven, eight, nine };

        return output.Split(' ').Select(SegmentBitsFromStr).Select(i => lookup.IndexOf(i)).Aggregate((sum, d) => sum * 10 + d);
    }

    protected override long? Part1()
        => Input.Lines().SelectMany(l => l.Split(" | ").Last().Split(' '))
            .Select(s => s.Length).Count(x => x is not 6 and not 5);

    protected override long? Part2() => Input.Lines().Sum(GetOutput);
}
