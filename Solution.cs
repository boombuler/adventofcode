using AdventOfCode.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public abstract class Solution : Solution<long?> { }
    public abstract class Solution<TSolution> : Solution<TSolution, TSolution> { }
    public abstract class Solution<TSolution1, TSolution2> : ISolution
    {
        private OutputMode fTextMode = new DefaultOut();


        private static readonly Regex TrailingInt = new Regex(@"\d+$", RegexOptions.Compiled);
        public virtual int Year => int.Parse(TrailingInt.Match(GetType().Namespace).Value); // Override when this throws an exception 
        public virtual int Day => int.Parse(TrailingInt.Match(GetType().Name).Value); // Override when this throws an exception 

        public Solution()
        {
            fInput = new Lazy<string>(() => ReadInput($"Input", $"{Day:d2}.txt"));
        }

        #region Puzzle Input
        private string ReadInput(string folder, string file) 
        {
            string relPath = Path.Combine(folder, Year.ToString(), file);
            return File.ReadAllText(relPath).TrimEnd('\r', '\n');
        }

        private readonly Lazy<string> fInput;
        protected string Input => fInput.Value;
        
        public string Sample(string suffix = null) => ReadInput("Samples", $"{Day:d2}{(!string.IsNullOrEmpty(suffix) ? ("-"+suffix) : string.Empty)}.txt");

        #endregion

        #region Execution
        protected abstract TSolution1 Part1();
        protected virtual TSolution2 Part2() => default;
        public void Run()
        {
            WriteLn<DefaultOut>($" ====================");
            WriteLn<DefaultOut>($" = Year {Year:d4} Day {Day:d2} =");
            WriteLn<DefaultOut>($" ====================");
            WriteLn<DefaultOut>(null);

            WriteLn<DefaultOut>("-- Part 1 --");
            var p1 = Part1();
            WriteLn<DefaultOut>(string.Format("              Solution : {0}", p1));
            WriteLn<DefaultOut>(null);
            WriteLn<DefaultOut>(null);

            if (Day < 25)
            {
                fAssertionCounter = 1;
                WriteLn<DefaultOut>("-- Part 2 --");

                var p2 = Part2();
                if (!Equals(p2, default(TSolution2)))
                    WriteLn<DefaultOut>(string.Format("              Solution : {0}", p2));
                WriteLn<DefaultOut>(string.Empty);
            }
        }

        #endregion

        #region Assertions

        private int fAssertionCounter = 1;
        private void WriteAssertion(string name, bool result)
        {
            if (string.IsNullOrEmpty(name))
                name = Convert.ToString(fAssertionCounter++);
            Console<AssertOut>().WriteResult(name, result);
        }

        protected void Assert<T>(T actual, T target, string name = null)
            => WriteAssertion(name, Equals(actual, target));

        protected void Assert(bool result, string name = null)
            => WriteAssertion(name, result);


        #endregion

        #region Console

        private T Console<T>()
            where T : OutputMode, new()
        {
            if (!(fTextMode is T))
            {
                fTextMode.Exit();
                fTextMode = new T();
                fTextMode.Enter();
            }
            return (T)fTextMode;
        }

        private void WriteLn<T>(object output)
            where T : OutputMode, new()
        => Console<T>().Write(Convert.ToString(output) + Environment.NewLine);

        protected void Debug(object output)
            => WriteLn<DebugOut>(output);

        protected void Error(string output)
            => WriteLn<ErrorOut>(output);

        #endregion
    }
}
