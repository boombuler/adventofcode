using AdventOfCode.Console;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    abstract class Solution : Solution<long?> { }
    abstract class Solution<TSolution> : Solution<TSolution, TSolution> { }
    abstract class Solution<TSolution1, TSolution2> : ISolution
    {
        private static readonly Regex TrailingInt = new Regex(@"\d+$", RegexOptions.Compiled);
        public virtual int Year => int.Parse(TrailingInt.Match(GetType().Namespace).Value); // Override when this throws an exception
        public virtual int Day => int.Parse(TrailingInt.Match(GetType().Name).Value); // Override when this throws an exception

        private IOutput fOutput;

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

        #endregion

        #region ISolution

        string ISolution.Part1(IOutput output) => Run(output, Part1);
        string ISolution.Part2(IOutput output) => Run(output, Part2);

        private string Run<T>(IOutput o, Func<T> actn)
        {
            var oldOut = fOutput;
            try
            {
                fOutput = o;
                return Convert.ToString(actn());
            }
            finally
            {
                fOutput = oldOut;
            }
        }

        #endregion

        #region Assertions

        protected void Error(string msg)
            => fOutput?.Error(msg);

        protected void Debug(object msg)
            => fOutput?.Debug(msg);

        protected void Assert<T>(T actual, T target, string name = null)
            => fOutput?.Assertion(name, Equals(actual, target), $"expected {target} got {actual}");

        protected void Assert(bool result, string name = null)
            => fOutput?.Assertion(name, result, null);

        #endregion
    }
}
