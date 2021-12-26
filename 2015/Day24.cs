namespace AdventOfCode._2015;

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day24 : Solution
{
    private static IEnumerable<long[]> SubsetSums(long[] arr, long sum)
    {
        var result = new List<long[]>();
        var values = new long[arr.Length];

        void WalkItems(int i, int k, long rest)
        {
            if (rest < 0)
                return;
            if (rest == 0)
            {
                var res = new long[i];
                Array.Copy(values, res, i);
                result.Add(res);
            }
            else
            {
                for (int j = k; j < arr.Length; j++)
                {
                    var value = arr[j];
                    values[i] = value;
                    WalkItems(i + 1, j + 1, rest - value);
                }
            }

        }
        WalkItems(0, 0, sum);
        return result;
    }

    private static long GetP1QE(string packageList, int grpCount)
    {
        var packages = packageList.Lines().Select(long.Parse).ToArray();
        var groupSize = packages.Sum();
        groupSize /= grpCount;

        static long QE(long[] vals) => vals.Aggregate((a, b) => a * b);

        var grp1 = SubsetSums(packages, groupSize).OrderBy(s => s.Length).ThenBy(QE).First();

        return QE(grp1);
    }

    protected override long? Part1()
    {
        Assert(GetP1QE(Sample(), 3), 99);
        return GetP1QE(Input, 3);
    }
    protected override long? Part2()
    {
        Assert(GetP1QE(Sample(), 4), 44);
        return GetP1QE(Input, 4);
    }
}
