using AdventHelper;
using System;
using System.Linq;
using System.Numerics;

namespace _2020_13
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private long GetNextDepatureId(string inputFile)
        {
            var lines = ReadLines(inputFile);
            var minDepatureTime = long.Parse(lines.First());
            var depaturePlan = lines.Last().Split(',').Where(id => id != "x")
                .Select(long.Parse)
                .Select(id => new { id, depatureTime = minDepatureTime + id - (minDepatureTime % id) });
            var bus = depaturePlan.OrderBy(p => p.depatureTime).First();
            return bus.id * (bus.depatureTime - minDepatureTime);
        }

        private long GetContestWinningMinute(string inputFile)
        {
            var busses = ReadLines(inputFile).Last().Split(',')
                .Select((s, i) => new { ID = s, idx = i })
                .Where(x => x.ID != "x")
                .Select(x => new { ID = long.Parse(x.ID), Idx = (long)x.idx });

            var result = busses
                .Select(b => (a: (BigInteger)((b.ID - b.Idx) % b.ID), n: (BigInteger)b.ID))
                .Aggregate(ChineseRemainder.Solve);
            if (result.a < 0)
                return (long)(result.a + result.n);
            return (long)result.a;
        }
        protected override long? Part1()
        {
            Assert(GetNextDepatureId("Sample"), 295);
            return GetNextDepatureId("Input");
        }

        protected override long? Part2()
        {
            Assert(ChineseRemainder.Solve((2, 8), (4, 5)).a, 34);
            Assert(GetContestWinningMinute("Sample"), 1068781);
            return GetContestWinningMinute("Input");
        }
    }
}
