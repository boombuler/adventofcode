using AdventHelper;
using System;
using System.IO;
using System.Linq;

namespace _2017_09
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private (long Score, long GarbageCount) ProcessStream(string str)
            => ProcessStream(new StringReader(str));

        private (long Score, long GarbageCount) ProcessStream(TextReader sr)
        {
            long score = 0;
            long garbageCount = 0;
            long nesting = 0;

            using(sr)
            {
                while (sr.TryRead(out char cur))
                {   
                    switch(cur)
                    {
                        case '{':
                            nesting++;
                            break;
                        case '}':
                            score += nesting;
                            nesting--;
                            break;
                        case '<':
                            {
                                bool garbage = true;
                                while (garbage)
                                {
                                    switch((char)sr.Read())
                                    {
                                        case '!': sr.Read(); break; // Skip next
                                        case '>': garbage = false; break;
                                        default: garbageCount++; break;
                                    }
                                }
                            }break;
                    }
                }
            }
            return (score, garbageCount);
        }

        protected override long? Part1()
        {
            Assert(ProcessStream("{}").Score, 1);
            Assert(ProcessStream("{{{}}}").Score, 6);
            Assert(ProcessStream("{{},{}}").Score, 5);
            Assert(ProcessStream("{{{},{},{{}}}}").Score, 16);
            Assert(ProcessStream("{<a>,<a>,<a>,<a>}").Score, 1);
            Assert(ProcessStream("{{<ab>},{<ab>},{<ab>},{<ab>}}").Score, 9);
            Assert(ProcessStream("{{<!!>},{<!!>},{<!!>},{<!!>}}").Score, 9);
            Assert(ProcessStream("{{<a!>},{<a!>},{<a!>},{<ab>}}").Score, 3);
            return ProcessStream(OpenResource("Input")).Score;
        }

        protected override long? Part2()
        {
            Assert(ProcessStream("<>").GarbageCount, 0);
            Assert(ProcessStream("<random characters>").GarbageCount, 17);
            Assert(ProcessStream("<<<<>").GarbageCount, 3);
            Assert(ProcessStream("<{!>}>").GarbageCount, 2);
            Assert(ProcessStream("<!!>").GarbageCount, 0);
            Assert(ProcessStream("<!!!>>").GarbageCount, 0);
            Assert(ProcessStream("<{o\"i!a,<{i<a>").GarbageCount, 10);
            return ProcessStream(OpenResource("Input")).GarbageCount;
        }
    }
}
