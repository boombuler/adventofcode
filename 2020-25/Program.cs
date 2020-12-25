using System;
using System.Linq;
using AdventHelper;

namespace _2020_25
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        const long MODULUS = 20201227;
        private long Loop(long subject, long value)
            => (subject * value) % MODULUS;

        private long GetEncryptionKey(long pub1, long pub2)
        {
            long subject = 1;
            while(true)
            {
                long value = 1;
                for (long loopCount = 1; loopCount <= MODULUS; loopCount++)
                {
                    value = Loop(subject, value);
                    if (value == pub1 || value == pub2)
                    {
                        subject = value == pub1 ? pub2 : pub1;
                        value = 1;
                        for (long i = 0; i < loopCount; i++)
                            value = Loop(subject, value);
                        return value;
                    }
                }
                subject++;
            }
        }

        protected override long? Part1()
        {
            Assert(GetEncryptionKey(5764801, 17807724), 14897079);
            return GetEncryptionKey(11562782, 18108497);
        }
    }
}
