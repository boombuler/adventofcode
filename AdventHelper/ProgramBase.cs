using AdventHelper.Console;
using System;
using System.Collections.Generic;
using System.IO;

namespace AdventHelper
{
    public abstract class ProgramBase : ProgramBase<long?>{}
    public abstract class ProgramBase<TSolution>: ProgramBase<TSolution, TSolution> { }
    public abstract class ProgramBase<TSolution1, TSolution2>
    {
        private OutputMode fTextMode = new DefaultOut();

        #region Puzzle Input
        protected StreamReader OpenResource(string fileName)
        {
            var type = GetType();
            var stream = type.Assembly.GetManifestResourceStream($"{type.Namespace}.{fileName}.txt");
            if (stream == null)
                throw new InvalidDataException();
            return new StreamReader(stream);
        }

        protected IEnumerable<string> ReadLines(string fileName)
        {
            using (var sr = OpenResource(fileName))
            {
                while (!sr.EndOfStream)
                    yield return sr.ReadLine();
            }
        }

        #endregion

        #region Execution
        protected abstract TSolution1 Part1();
        protected virtual TSolution2 Part2() => default;
        public void Run()
        {
            WriteLn<DefaultOut>("-- Part 1 --");
            var p1 = Part1();
            WriteLn<DefaultOut>(string.Format("              Solution : {0}", p1));
            WriteLn<DefaultOut>(null);
            WriteLn<DefaultOut>(null);

            fAssertionCounter = 1;
            WriteLn<DefaultOut>("-- Part 2 --");

            var p2 = Part2();
            if (!Equals(p2, default(TSolution2)))
                WriteLn<DefaultOut>(string.Format("              Solution : {0}", p2));
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
