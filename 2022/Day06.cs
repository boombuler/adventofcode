namespace AdventOfCode._2022;

class Day06 : Solution
{
    private int PacketStart(string s, int cnt = 4)
        => s.SlidingWindow(cnt)
            .Select((c, i) => new { Count = c.Distinct().Count(), Index = i })
            .Where(c => c.Count == cnt)
            .First().Index + cnt;

    protected override long? Part1()
    {
        Assert(PacketStart("mjqjpqmgbljsphdztnvjfqwrcgsmlb"), 7);
        Assert(PacketStart("bvwbjplbgvbhsrlpgdmjqwftvncz"), 5);
        Assert(PacketStart("nppdvjthqldpwncqszvftbrmjlhg"), 6);
        Assert(PacketStart("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg"), 10);
        Assert(PacketStart("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"), 11);
        return PacketStart(Input);
    }

    protected override long? Part2() => PacketStart(Input, 14);
}
