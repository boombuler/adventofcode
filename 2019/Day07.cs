using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

namespace AdventOfCode._2019
{
    class Day07 : Solution
    {
        class IntCodeVMParam : IEnumerable<long>
        {
            class Enumerator : IEnumerator<long>
            {
                private long fCurrent = 0;
                private readonly Queue<long> fQueue;
                public Enumerator(Queue<long> queue)
                    => fQueue = queue;

                public long Current => fCurrent;
                object IEnumerator.Current => fCurrent;

                public void Dispose() { }

                public bool MoveNext() => fQueue.TryDequeue(out fCurrent);

                public void Reset() { }
            }

            private readonly Queue<long> fQueue = new Queue<long>();

            public void AddParam(long l)
                => fQueue.Enqueue(l);

            public IntCodeVMParam(params long[] initialParams)
            {
                foreach(var p in initialParams)
                    AddParam(p);
            }

            public IEnumerator<long> GetEnumerator() => new Enumerator(fQueue);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private long MaxThrusterSignal(string code)
        {
            var phaseSettings = new long[] { 0, 1, 2, 3, 4 };
            
            return phaseSettings.Permuatate()
                .Select(settings => settings.Aggregate((long)0, (o, phase) => new IntCodeVM(code).Run(new[] { phase, o }).First()))
                .Max();
        }

        private long RunWithFeedbackLoop(string code, long[] phaseSettings)
        {
            var parameters = phaseSettings.Select(p => new IntCodeVMParam(p)).ToArray();
            var vms = parameters.Select(p => new IntCodeVM(code).Run(p).GetEnumerator()).ToArray();
            long param = 0;
            int amp = 0;
            while (true)
            {
                parameters[amp].AddParam(param);
                if (!vms[amp].MoveNext())
                    return param;
                param = vms[amp].Current;
                amp = (amp + 1) % vms.Length;
            }
        }

        private long MaxThrusterSignalFeedbackLoop(string code)
        {
            var phaseSettings = new long[] { 5, 6, 7, 8, 9 };

            return phaseSettings.Permuatate()
                .Select(ps => RunWithFeedbackLoop(code, ps))
                .Max();
        }


        protected override long? Part1()
        {
            Assert(MaxThrusterSignal("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0"), 43210);
            Assert(MaxThrusterSignal("3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0"), 54321);
            Assert(MaxThrusterSignal("3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0"), 65210);

            return MaxThrusterSignal(Input);
        }

        protected override long? Part2()
        {
            Assert(MaxThrusterSignalFeedbackLoop("3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5"), 139629729);
            Assert(MaxThrusterSignalFeedbackLoop("3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10"), 18216);
            return MaxThrusterSignalFeedbackLoop(Input);
        }
    }
}
