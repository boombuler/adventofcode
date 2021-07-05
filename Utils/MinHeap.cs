using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    class MinHeap<T>
    {
        private int fCount;
        private T[] fData;
        private readonly IComparer<T> fComparer;

        public int Count => fCount;

        public MinHeap(IComparer<T> comparer = null)
        {
            fCount = 0;
            fData = new T[64];
            fComparer = comparer ?? Comparer<T>.Default;
        }

        private int IParent(int idx) => ((idx + 1) / 2) - 1;
        private int ILeft(int idx) => ((idx + 1) * 2) - 1;
        private int IRight(int idx) => (idx + 1) * 2;

        public void Push(T value)
        {
            if (fData.Length == fCount)
                Array.Resize(ref fData, fCount * 2);

            int idx = fCount++;
            while (idx > 0 && fComparer.Compare(value, fData[IParent(idx)]) < 0)
            {
                var iPar = IParent(idx);
                fData[idx] = fData[iPar];
                idx = iPar;
            }
            fData[idx] = value;
        }

        public bool TryPop(out T val)
        {
            if (fCount > 0)
            {
                val = Pop();
                return true;
            }
            val = default;
            return false;
        }

        public T Pop()
        {
            if (fCount == 0)
                throw new InvalidOperationException();

            bool LessThen(int ia, int ib) => fComparer.Compare(fData[ia], fData[ib]) < 0;

            var result = fData[0];
            fData[0] = fData[--fCount];

            int idx = 0;
            while (ILeft(idx) < fCount)
            {
                var (il, ir) = (ILeft(idx), IRight(idx));

                var iMin = (ir >= fCount || LessThen(il, ir)) ? il : ir;

                if (LessThen(idx, iMin))
                    break;
                (fData[iMin], fData[idx]) = (fData[idx], fData[iMin]);
                idx = iMin;
            }
            fData[fCount] = default;
            return result;
        }
    }


    class MaxHeap<T> : MinHeap<T>
    {
        public MaxHeap(IComparer<T> comparer = null)
            : base((comparer ?? Comparer<T>.Default).Invert())
        {
        }
    }
}
