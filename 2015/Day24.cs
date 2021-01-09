using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2015
{
    class Day24 : Solution
    {
        private static IEnumerable<long[]> SubsetSums(long[] arr, long sum)
        {
            List<long[]> result = new List<long[]>();
            int[] indices = new int[arr.Length];
            
            void WalkItems(int i, long rest)
            {
                if (rest == 0)
                    result.Add(Enumerable.Range(0, i).Select(j => arr[indices[j]]).ToArray());
                else
                {
                    int k = (i != 0) ? indices[i - 1] + 1 : 0;
                    for (int j = k; j < arr.Length; j++)
                    {
                        indices[i] = j;
                        WalkItems(i + 1, rest - arr[j]);
                    }
                }
                
            }
            WalkItems(0, sum);
            return result;
        }

        private long GetP1QE(string packageList, int grpCount)
        {
            var packages = packageList.Lines().Select(long.Parse).ToArray();
            var groupSize = packages.Sum();
            groupSize /= grpCount;

            long QE(long[] vals) => vals.Aggregate((a, b) => a * b);

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
}
