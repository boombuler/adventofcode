namespace AdventOfCode._2022;

class Day06 : Solution
{
    int PacketStart(string s, int cnt = 4)
        => s.SlidingWindow(cnt)
            .Select((c, i) => new { Count = c.Distinct().Count(), Index = i })
            .Where(c => c.Count == cnt)
            .First().Index + cnt;

    protected override long? Part1()
    {
        Assert(7, PacketStart("mjqjpqmgbljsphdztnvjfqwrcgsmlb"));
        Assert(5, PacketStart("bvwbjplbgvbhsrlpgdmjqwftvncz"));
        Assert(6, PacketStart("nppdvjthqldpwncqszvftbrmjlhg"));
        Assert(10, PacketStart("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg"));
        Assert(11, PacketStart("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"));
        return PacketStart(Input);
    }

    protected override long? Part2() => PacketStart(Input, 14);
}
