using System;
using AdventHelper;

namespace _2016_19
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();
        private const long INPUT = 3_018_458;
        private long Circle(long n)
        {
            var b = (long)Math.Pow(2, Math.Floor(Math.Log(n) / Math.Log(2)));
            var l = n - b;
            return 2*l+1;
        }

        public static long Cross(long n)
        {
            var b = (long)Math.Pow(3, Math.Floor(Math.Log(n) / Math.Log(3)));
            if (n == b)
                return n;
            if (n - b <= b)
                return n - b;
            return 2 * n - 3 * b;
        }

        protected override long? Part1()
        {
            Assert(Circle(5), 3);
            Assert(Circle(6), 5);
            Assert(Circle(13), 11);
            return Circle(INPUT);
        }

        

        protected override long? Part2()
        {
            Assert(Cross(5), 2);
            return Cross(INPUT);
        }
    }
}
