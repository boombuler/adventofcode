namespace AdventOfCode.Utils.Parser;

ref struct Result<TResult>
{
    private readonly TResult fResult;
    private readonly string fError;
    private readonly bool fSuccess;

    public Input Input { get; private set; }

    private Result(string error)
    {
        fResult = default;
        fError = error;
        fSuccess = false;
        Input = new Input(string.Empty);
    }
    public Result(TResult result, Input input)
    {
        fResult = result;
        fError = default;
        fSuccess = true;
        Input = input;
    }

    public readonly Result<TValue> Map<TValue>(Func<TResult, TValue> map)
    {
        if (fSuccess)
            return new Result<TValue>(map(fResult), Input);
        return Result<TValue>.Failed(fError);
    }

    public readonly bool HasValue => fSuccess;

    public readonly TResult Value => fSuccess ? fResult : throw new InvalidOperationException(fError);

    public readonly string Error => !fSuccess ? fError : throw new InvalidOperationException();

    public static implicit operator Result<TResult>(string error)
        => Failed(error);

    public static Result<TResult> Failed(string error)
        => new(error);
}
