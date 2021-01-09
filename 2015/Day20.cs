using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2015
{
    class Day20 : Solution
    {
        private IEnumerable<long> GetElves(long house)
        {
            // till square root 
            for (long e = 1; e <= Math.Sqrt(house); e++)
            {
                if (house % e == 0)
                {
                    yield return e;
                    var other = house / e;
                    if (other != e)
                        yield return other;
                }
            }
        }

        private long MinHouseNoEndlessHouses(int presents)
            => Enumerable.Range(1, presents)
                .Where(h => GetElves(h).Select(e => e * 10).Sum() >= presents)
                .First();

        private long MinHouseNo(int presents)
        {
            int house = 1;

            IEnumerable<int> GetElves()
            {
                // till square root 
                for (int e = 1; e <= Math.Sqrt(house); e++)
                {
                    if (house % e == 0)
                    {
                        yield return e;
                        var other = house / e;
                        if (other != e)
                            yield return other;
                    }
                }
            }

            var delivered = new List<int>();

            while (true)
            {
                var sum = 0;
                foreach (var e in GetElves())
                {
                    if (delivered.Count < e)
                        delivered.Add(1);
                    else if (delivered[e-1] <= 50)
                        delivered[e-1] = delivered[e-1] + 1;
                    else
                        continue;
                    sum += e * 11;
                }
               
                if (sum >= presents)
                    return house;
                house++;
            }
        }

        protected override long? Part1()
        {
            Assert(MinHouseNoEndlessHouses(150), 8);
            return MinHouseNoEndlessHouses(int.Parse(Input));
        }

        protected override long? Part2()
        {
            return MinHouseNo(int.Parse(Input));
        }

    }
}
