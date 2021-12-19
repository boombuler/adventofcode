using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    static class FunctionalHelpers
    {
        public static Func<T1, T3> Combine<T1, T2, T3>(this Func<T1, T2> first, Func<T2, T3> second)
            => (T1 v) => second(first(v));
    }
}
