using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    static class ComparerBuilder<T>
    {
        public static IComparer<T> CompareBy<TVal>(Func<T, TVal> selector)
        {
            var c = Comparer<TVal>.Default;
            return Comparer<T>.Create((a, b) => c.Compare(selector(a), selector(b)));
        }

        public static IComparer<T> CompareByDesc<TVal>(Func<T, TVal> selector)
        {
            var c = Comparer<TVal>.Default;
            return Comparer<T>.Create((a, b) => -c.Compare(selector(a), selector(b)));
        }
    }

    static class ComparerExtensions
    {
        public static IComparer<T> Invert<T>(this IComparer<T> comp)
           => Comparer<T>.Create((a, b) => -comp.Compare(a, b));

        public static IComparer<T> ThenBy<T, TVal>(this IComparer<T> comp, Func<T, TVal> selector)
        {
            var c = Comparer<TVal>.Default;
            return Comparer<T>.Create((a, b) => {
                var r = comp.Compare(a, b);
                return r != 0 ? r : c.Compare(selector(a), selector(b));
            });
        }


        public static IComparer<T> ThenByDesc<T, TVal>(this IComparer<T> comp, Func<T, TVal> selector)
        {
            var c = Comparer<TVal>.Default;
            return Comparer<T>.Create((a, b) => {
                var r = comp.Compare(a, b);
                return r != 0 ? r : -c.Compare(selector(a), selector(b));
            });
        }
    }
}
