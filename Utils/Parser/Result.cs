namespace AdventOfCode.Utils.Parser;

ref struct Result<TResult>
{
    private readonly TResult fResult;
    private readonly string fError;
    private readonly bool fSuccess;

    private Result(TResult result, string error, bool success)
    {
        fResult = result;
        fError = error;
        fSuccess = success;
    }

    public Result<TValue> Map<TValue>(Func<TResult, TValue> map)
    {
        if (fSuccess)
            return map(fResult);
        return Result<TValue>.Failed(fError);
    }

    public bool HasValue => fSuccess;

    public TResult Value => fSuccess ? fResult : throw new InvalidOperationException(fError);

    public string Error => !fSuccess ? fError : throw new InvalidOperationException();
    public TResult GetValueOrDefault(TResult fallback = default) => fSuccess ? fResult : fallback;


    public static implicit operator Result<TResult>(TResult result)
        => Success(result);

    public static implicit operator Result<TResult>(string error)
        => Failed(error);


    public static Result<TResult> Success(TResult result)
        => new Result<TResult>(result, default, true);

    public static Result<TResult> Failed(string error)
        => new Result<TResult>(default, error, false);
}
