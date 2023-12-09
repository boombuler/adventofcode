namespace AdventOfCode.Utils;

class Disposable(Action onDispose) : IDisposable
{
    private bool fDisposed;
    public void Dispose()
    {
        if (!fDisposed)
        {
            fDisposed= true;
            onDispose?.Invoke();
        }
    }
}
