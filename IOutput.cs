namespace AdventOfCode;
interface IOutput
{
    void Debug(object data);
    void Error(string data);
    void Assertion(string? name, bool result, string errorTxt);
}
