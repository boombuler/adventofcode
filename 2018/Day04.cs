namespace AdventOfCode._2018;

using System.Globalization;

class Day04 : Solution
{
    record SleepingMinutes(int Minute, int Guard);
    private static readonly Regex BeginShift = new(@"\d+", RegexOptions.Compiled);
    private static IEnumerable<SleepingMinutes> ParseInput(string input)
    {
        int curGuard = 0;
        var curMinute = 0;
        bool awake = true;
        var result = new List<SleepingMinutes>();
        foreach (var log in input.Lines().OrderBy(l => l))
        {
            var t = DateTime.ParseExact(log.Substring(1, 16), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            var action = log[19..];
            var beginShift = BeginShift.Match(action);
            if (beginShift.Success)
            {
                if (t.Hour > 0)
                    curMinute = 0;
                else
                    curMinute = t.Minute;
                curGuard = int.Parse(beginShift.Value);
                awake = true;
            }
            else
            {
                if (!awake)
                    result.AddRange(Enumerable.Range(curMinute, t.Minute - curMinute).Select(m => new SleepingMinutes(m, curGuard)));
                curMinute = t.Minute;
                awake = action == "wakes up";
            }
        }
        return result;
    }

    private static long Strategy1(string input)
        => ParseInput(input)
            .GroupBy(m => m.Guard).OrderByDescending(g => g.Count())
            .First()
            .GroupBy(m => m.Minute)
            .OrderByDescending(g => g.Count())
            .Select(g => g.First())
            .Select(m => m.Guard * m.Minute)
            .First();

    private static long Strategy2(string input)
        => ParseInput(input)
            .GroupBy(m => new { m.Guard, m.Minute })
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key.Guard * g.Key.Minute)
            .First();

    protected override long? Part1()
    {
        Assert(Strategy1(Sample()), 240);
        return Strategy1(Input);
    }

    protected override long? Part2()
    {
        Assert(Strategy2(Sample()), 4455);
        return Strategy2(Input);
    }
}
