using AdventHelper.Console;
using System;
using System.Collections.Generic;
using System.IO;

namespace AdventHelper
{
    public abstract class ProgramBase : ProgramBase<long?>{}
    public abstract class ProgramBase<TSolution>
    {
        private OutputMode fTextMode = new DefaultOut();

        #region Puzzle Input
        protected Stream OpenResource(string fileName)
        {
            var type = GetType();
            var stream = type.Assembly.GetManifestResourceStream($"{type.Namespace}.{fileName}");
            if (stream == null)
                throw new InvalidDataException();
            return stream;
        }

        protected IEnumerable<string> ReadLines(string fileName)
        {
            using (var file = OpenResource(fileName + ".txt"))
            {
                using (var sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                        yield return sr.ReadLine();
                }
            }
        }

        #endregion

        #region Execution
        protected abstract TSolution Part1();
        protected virtual TSolution Part2() => default;
        public void Run()
        {
            WriteLn<DefaultOut>("-- Part 1 --");
            var p = Part1();
            WriteLn<DefaultOut>(string.Format("              Solution : {0}", p));
            WriteLn<DefaultOut>(null);
            WriteLn<DefaultOut>(null);

            fAssertionCounter = 1;
            WriteLn<DefaultOut>("-- Part 2 --");

            p = Part2();
            if (!Equals(p, default(TSolution)))
                WriteLn<DefaultOut>(string.Format("              Solution : {0}", p));
            WriteLn<DefaultOut>(string.Empty);
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
            where T: OutputMode, new()
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
