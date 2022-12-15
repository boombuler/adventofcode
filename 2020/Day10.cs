namespace AdventOfCode._2020;

using System.Buffers;

class Day10 : Solution
{
    private class ChainCache
    {
        private readonly Dictionary<string, long> fData = new();
        private static string Key(ReadOnlySpan<long> span)
        {
            var sb = new StringBuilder(span.Length * 4);
            for (int i = 0; i < span.Length; i++)
                sb.Append('|').Append(span[i]);
            return sb.ToString();
        }
        public long GetOrAdd(ReadOnlySpan<long> part, Func<long> calc)
        {
            var key = Key(part);
            if (fData.TryGetValue(key, out long val))
                return val;
            return fData[key] = calc();
        }
    }

    private static long[] GetDifferences(string inputs)
    {
        var adapterChain = inputs.Lines().Select(long.Parse).Order();
        var result = new long[4] { 0, 0, 0, 1 }; // Add last adapter to device
        long lastJoltage = 0;
        foreach (var adapter in adapterChain)
        {
            var dj = adapter - lastJoltage;
            lastJoltage = adapter;
            if (dj > 3)
                throw new Exception("Invalid Adapter Chain");
            result[dj] = result[dj] + 1;
        }
        return result;
    }

    private long CoundValidAdapterChains(string inputs)
    {
        var adapterChain = inputs.Lines().Select(long.Parse).Order().ToArray();
        return CoundValidAdapterChains(adapterChain, 0, adapterChain.Length, new ChainCache());
    }

    private long CoundValidAdapterChains(long[] chain, int idx, int len, ChainCache cache)
    {
        long Value(int idx) => idx < 0 ? 0 : chain[idx];

        if (idx != len - 1) // keep last adapter
        {
            var ck = chain.AsSpan().Slice(idx + 1, len - idx - 1);
            long result = cache.GetOrAdd(ck, () => CoundValidAdapterChains(chain, idx + 1, len, cache));

            if (Value(idx + 1) - Value(idx - 1) < 4)
            {
                var subLen = len - 1;
                var arr = ArrayPool<long>.Shared.Rent(subLen);
                Array.Copy(chain, arr, idx);
                Array.Copy(chain, idx + 1, arr, idx, subLen - idx);
                result += CoundValidAdapterChains(arr, idx, subLen, cache);
                ArrayPool<long>.Shared.Return(arr);
            }

            return result;
        }

        return 1;
    }

    protected override long? Part1()
    {
        Assert(string.Join(",", GetDifferences(Sample("1"))), "0,7,0,5");
        Assert(string.Join(",", GetDifferences(Sample("2"))), "0,22,0,10");
        var diff = GetDifferences(Input);
        return diff[1] * diff[3];
    }

    protected override long? Part2()
    {
        Assert(CoundValidAdapterChains(Sample("1")), 8);
        Assert(CoundValidAdapterChains(Sample("2")), 19208);
        return CoundValidAdapterChains(Input);
    }
}
