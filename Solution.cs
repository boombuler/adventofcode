using AdventOfCode.Console;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    abstract class Solution : Solution<long?> { }
    abstract class Solution<TSolution> : Solution<TSolution, TSolution> { }
    abstract class Solution<TSolution1, TSolution2> : ScreenBase, ISolution
    {
        private static readonly Regex TrailingInt = new Regex(@"\d+$", RegexOptions.Compiled);
        public virtual int Year => int.Parse(TrailingInt.Match(GetType().Namespace).Value); // Override when this throws an exception
        public virtual int Day => int.Parse(TrailingInt.Match(GetType().Name).Value); // Override when this throws an exception

        public Solution()
        {
            fInput = new Lazy<string>(() => LoadInput());
        }

        #region Puzzle Input

        private string LoadInput()
        {
            const string sessionCookieFile = "Session.user";
            string relPath = Path.Combine("Input", Year.ToString(), $"{Day:d2}.txt");
            if (!File.Exists(relPath) && File.Exists(sessionCookieFile))
            {
                var request = (HttpWebRequest)WebRequest.Create($"https://adventofcode.com/{Year:d4}/day/{Day}/input");
                request.Method = "GET";
                request.Accept = "*/*";
                request.AllowAutoRedirect = false;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.0.3705)";
                request.CookieContainer = new CookieContainer();
                request.Credentials = null;
                request.CookieContainer.Add(new Cookie("session", File.ReadAllText(sessionCookieFile), "/", ".adventofcode.com"));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var inputData = response.GetResponseStream())
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(relPath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(relPath));
                        using (var fs = File.Create(relPath))
                            inputData.CopyTo(fs);
                    }
                }

            }
            return File.ReadAllText(relPath).TrimEnd('\r', '\n');
        }

        private readonly Lazy<string> fInput;
        protected string Input => fInput.Value;

        public virtual string Sample(string suffix = null)
        {
            var asm = GetType().Assembly;
            if (string.IsNullOrEmpty(suffix))
                suffix = string.Empty;
            else
                suffix = "-" + suffix;
            using (var stream = asm.GetManifestResourceStream(asm.GetName().Name + $".Samples._{Year:d4}.{Day:d2}{suffix}.txt"))
            {
                if (stream == null)
                {
                    Error("Sample could not be loaded!");
                    return string.Empty;
                }

                using (var sr = new StreamReader(stream))
                    return sr.ReadToEnd().TrimEnd('\r', '\n');
            }
        }

        #endregion

        #region Execution
        protected abstract TSolution1 Part1();
        protected virtual TSolution2 Part2() => default;
        public void Run()
        {
            try
            {
                WriteLn<DefaultOut>($" ====================");
                WriteLn<DefaultOut>($" = Year {Year:d4} Day {Day:d2} =");
                WriteLn<DefaultOut>($" ====================");
                WriteLn<DefaultOut>(null);

                WriteLn<DefaultOut>("-- Part 1 --");
                fAssertionHandler = new ConsoleAsssertionHandler(Console<AssertOut>);
                var p1 = Part1();
                WriteLn<DefaultOut>(string.Format("              Solution : {0}", p1));
                WriteLn<DefaultOut>(null);
                WriteLn<DefaultOut>(null);

                TSolution2 p2 = default;
                if (Day < 25)
                {
                    fAssertionHandler = new ConsoleAsssertionHandler(Console<AssertOut>);
                    WriteLn<DefaultOut>("-- Part 2 --");

                    p2 = Part2();
                    if (!Equals(p2, default(TSolution2)))
                        WriteLn<DefaultOut>(string.Format("              Solution : {0}", p2));
                    WriteLn<DefaultOut>(string.Empty);
                }
                WriteResults(Convert.ToString(p1), Convert.ToString(p2));
            }
            catch(Exception e)
            {
                Error(e.Message);
            }
        }

        #endregion

        #region Assertions

        interface IAssertionHandler
        {
            void HandleAssertion(string name, bool result, string errorTxt);
        }

        class ConsoleAsssertionHandler : IAssertionHandler
        {
            private readonly Func<AssertOut> fGetConsole;
            private int fAssertionCounter = 1;
            public ConsoleAsssertionHandler(Func<AssertOut> getConsole)
                => fGetConsole = getConsole;

            public void HandleAssertion(string name, bool result, string errorTxt)
            {
                if (string.IsNullOrEmpty(name))
                    name = Convert.ToString(fAssertionCounter++);
                fGetConsole().WriteResult(name, result, errorTxt);
            }
        }

        class ExceptionAssertionHandler : IAssertionHandler
        {
            public void HandleAssertion(string name, bool result, string errorTxt)
            {
                if (!result)
                    throw new InvalidOperationException(name);
            }
        }

        private IAssertionHandler fAssertionHandler;
        private void WriteAssertion(string name, bool result, string errorTxt)
        {
            fAssertionHandler.HandleAssertion(name, result, errorTxt);
        }

        protected void Assert<T>(T actual, T target, string name = null)
            => WriteAssertion(name, Equals(actual, target), $"expected {target} got {actual}");

        protected void Assert(bool result, string name = null)
            => WriteAssertion(name, result, null);


        #endregion

        #region Console

        private void WriteLn<T>(object output)
            where T : OutputMode, new()
        => Console<T>().Write(Convert.ToString(output) + Environment.NewLine);

        protected void Debug(object output)
            => WriteLn<DebugOut>(output);

        protected void Error(string output)
            => WriteLn<ErrorOut>(output);

        #endregion

        #region Validate

        const string RESULT_DIR = "Results";

        private void WriteResults(string p1, string p2)
        {
            if (!Directory.Exists("Results"))
                return;
            Directory.CreateDirectory(Path.Combine(RESULT_DIR, Year.ToString()));
            string relPath = Path.Combine(RESULT_DIR, Year.ToString(), $"{Day:d2}.txt");
            File.WriteAllText(relPath, p1 + "\n" + p2);
        }

        private (string Part1, string Part2) Results()
        {
            string relPath = Path.Combine(RESULT_DIR, Year.ToString(), $"{Day:d2}.txt");
            if (!File.Exists(relPath))
                return (null, null);
            using (var sr = new StreamReader(relPath))
                return (sr.ReadLine(), sr.ReadLine());
        }

        public bool Validate()
        {
            var oldHandler = fAssertionHandler;
            var counter = new ExceptionAssertionHandler();
            fAssertionHandler = counter;
            try
            {
                var (res1, res2) = Results();

                var p1 = Part1();
                if (!string.IsNullOrEmpty(res1) && Convert.ToString(p1) != res1)
                    return false;
                if (Day < 25)
                {
                    var p2 = Part2();
                    if (!string.IsNullOrEmpty(res2) && Convert.ToString(p2) != res2)
                        return false;
                }
                return true;
            }
            catch { return false; }
            finally
            {
                fAssertionHandler = oldHandler;
            }
        }

        #endregion
    }
}
