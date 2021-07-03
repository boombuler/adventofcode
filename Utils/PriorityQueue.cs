using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    class PriorityQueue<T>
    {
        private int fCount;
        private T[] fHeap;
        private readonly IComparer<T> fComparer;

        public int Count => fCount;

        public PriorityQueue(IComparer<T> comparer = null)
        {
            fCount = 0;
            fHeap = new T[64];
            fComparer = comparer ?? Comparer<T>.Default;
        }

        private int IParent(int idx) => ((idx + 1) / 2) - 1;
        private int ILeft(int idx) => ((idx + 1) * 2) - 1;
        private int IRight(int idx) => (idx + 1) * 2;


        private bool LessThen(int ia, int ib) => fComparer.Compare(fHeap[ia], fHeap[ib]) < 0;

        public void Push(T value)
        {
            if (fHeap.Length == fCount)
                Array.Resize(ref fHeap, fCount * 2);

            int idx = fCount++;

            while (idx > 0 && LessThen(idx, IParent(idx)))
            {
                var iPar = IParent(idx);
                fHeap[idx] = fHeap[iPar];
                idx = iPar;
            }
            fHeap[idx] = value;
        }


        public T Pop()
        {
            if (fCount == 0)
                throw new InvalidOperationException();

            var result = fHeap[0];
            fHeap[0] = fHeap[--fCount];

            int idx = 0;
            while (ILeft(idx) < fCount)
            {
                var (il, ir) = (ILeft(idx), IRight(idx));

                var iMin = (ir >= fCount || LessThen(il, ir)) ? il : ir;

                if (LessThen(idx, iMin))
                    break;
                (fHeap[iMin], fHeap[idx]) = (fHeap[idx], fHeap[iMin]);
                idx = iMin;
            }
            return result;
        }

    }
}
