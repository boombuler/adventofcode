namespace AdventOfCode.Utils;

using System.Runtime.CompilerServices;

class MinHeap<T>
{
    private int fCount;
    private T[] fData;
    private readonly IComparer<T> fComparer;

    public int Count => fCount;

    public MinHeap(IComparer<T> comparer = null)
        : this(Enumerable.Empty<T>(), comparer)
    {
    }

    public MinHeap(IEnumerable<T> items, IComparer<T> comparer = null)
    {
        fData = items.ToArray();
        fCount = fData.Length;
        fComparer = comparer ?? Comparer<T>.Default;

        for (int i = IParent(fCount - 1); i >= 0; i--)
            Heapify(i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IParent(int idx) => (idx - 1) / 2;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ILeft(int idx) => (idx * 2) + 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IRight(int idx) => (idx * 2) + 2;

    public void Push(T value)
    {
        if (fData.Length == fCount)
            Array.Resize(ref fData, Math.Max(64, fCount * 2));

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

        var result = fData[0];
        fData[0] = fData[--fCount];

        Heapify(0);

        fData[fCount] = default;
        return result;
    }

    private void Heapify(int idx)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool LessThen(int ia, int ib) => fComparer.Compare(fData[ia], fData[ib]) < 0;

        while (ILeft(idx) < fCount)
        {
            var (il, ir) = (ILeft(idx), IRight(idx));

            var iMin = (ir >= fCount || LessThen(il, ir)) ? il : ir;

            if (LessThen(idx, iMin))
                break;
            (fData[iMin], fData[idx]) = (fData[idx], fData[iMin]);
            idx = iMin;
        }
    }
}

class MaxHeap<T> : MinHeap<T>
{
    public MaxHeap(IComparer<T> comparer = null)
        : this(Enumerable.Empty<T>(), comparer)
    {
    }

    public MaxHeap(IEnumerable<T> items, IComparer<T> comparer = null)
        : base(items, (comparer ?? Comparer<T>.Default).Invert())
    {
    }
}
