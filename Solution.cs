namespace AdventOfCode;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

abstract class Solution : Solution<long?> { }
abstract class Solution<TSolution> : Solution<TSolution, TSolution> { }
abstract class Solution<TSolution1, TSolution2> : ISolution
{
    private static readonly Regex TrailingInt = new(@"\d+$", RegexOptions.Compiled);
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
        const string SESSION_COOKIE_FILE = "Session.user";
        string relPath = Path.Combine("Input", Year.ToString(), $"{Day:d2}.txt");
        if (!File.Exists(relPath) && File.Exists(SESSION_COOKIE_FILE))
        {
            var baseAddress = new Uri("https://adventofcode.com");
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("session", File.ReadAllText(SESSION_COOKIE_FILE), "/", ".adventofcode.com"));

            using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            using var client = new HttpClient(handler) { BaseAddress = baseAddress };

            var response = client.GetAsync($"/{Year:d4}/day/{Day}/input").Result;
            if (response.IsSuccessStatusCode)
            {
                using var inputData = response.Content.ReadAsStream();
                if (!Directory.Exists(Path.GetDirectoryName(relPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(relPath));
                using var fs = File.Create(relPath);
                inputData.CopyTo(fs);
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
        using var stream = asm.GetManifestResourceStream(asm.GetName().Name + $".Samples._{Year:d4}.{Day:d2}{suffix}.txt");
        if (stream == null)
        {
            Error("Sample could not be loaded!");
            return string.Empty;
        }

        using var sr = new StreamReader(stream);
        return sr.ReadToEnd().TrimEnd('\r', '\n');
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
