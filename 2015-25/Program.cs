using AdventHelper;
using System;
using System.Linq;
using System.Collections.Generic;

namespace _2015_25
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private IEnumerable<(int column, int row)> GeneratePositions()
        {
            int startRow = 0;

            while (true)
            {
                startRow++;

                int col = 1;
                for (int row = startRow; row > 0; row --)
                {
                    yield return (col++, row);
                }
            }
        }

        private IEnumerable<long> GenerateCodes()
        {
            long value = 20151125;

            while(true)
            {
                yield return value;
                value = (value * 252533) % 33554393;
            }
        }

        private IEnumerable<(int column, int row, long code)> GenerateTable()
            => GeneratePositions().Zip(GenerateCodes(), (pos, val) => (pos.column, pos.row, val));

        private long LookupCode(int column, int row)
            => GenerateTable().Where(x => x.column == column && x.row == row).Select(x => x.code).First();
        
        protected override long? Part1() 
        {
            Assert(LookupCode(4, 4), 9380097);
            return LookupCode(3019, 3010);
        }

    }
}
