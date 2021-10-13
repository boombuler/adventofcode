using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2019
{
    class LazyIntCodeVMParam : IEnumerable<long>
    {
        class Enumerator : IEnumerator<long>
        {
            private readonly Func<long> fGetCurrentValue;
            public Enumerator(Func<long> getCurrentValue)
            {
                fGetCurrentValue = getCurrentValue;
            }

            public long Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                Current = fGetCurrentValue();
                return true;
            }

            public void Dispose() { }
            public void Reset() { }
        }

        private readonly Func<long> fGetCurrentValue;
        public LazyIntCodeVMParam(Func<long> getCurrentValue)
        {
            fGetCurrentValue = getCurrentValue;
        }

        public IEnumerator<long> GetEnumerator() => new Enumerator(fGetCurrentValue);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
