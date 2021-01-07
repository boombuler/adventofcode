using System;
using System.Collections.Generic;
using System.Linq;
using AdventHelper;

namespace _2017_13
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private IEnumerable<(Func<int, bool> passes, int severity)> ReadLayerFile(string layerFile)
        {
            foreach (var line in ReadLines(layerFile))
            {
                var parts = line.Split(':').Select(p => int.Parse(p.Trim())).ToArray();
                int layer = parts[0];
                int depth = parts[1];
                int mod = (depth - 1) * 2;
                var passes = new Func<int, bool>(offset => ((-offset - layer) % mod) != 0);

                yield return (passes, layer * depth);
            }
        }

        private long GetSeverity(string layerFile)
            => ReadLayerFile(layerFile).Where(l => !l.passes(0)).Sum(l => l.severity);


        private long GetDelayTime(string layerFile)
            => ReadLayerFile(layerFile).Select(l => l.passes)
                .Aggregate(EnumerableHelper.Generate(), (sequence, filter) => sequence.Where(filter))
                .First();
           

        protected override long? Part1()
        {
            Assert(GetSeverity("Sample"), 24);
            return GetSeverity("Input");
        }

        protected override long? Part2()
        {
            Assert(GetDelayTime("Sample"), 10);
            return GetDelayTime("Input");
        }
    }
}
