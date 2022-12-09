namespace AdventOfCode.Utils;

class Disposable : IDisposable
{
    private readonly Action fOnDispose;    
    private bool fDisposed;
    public Disposable(Action onDispose)
    {
        fOnDispose = onDispose;
    }
    public void Dispose()
    {
        if (!fDisposed)
        {
            fDisposed= true;
            fOnDispose?.Invoke();
        }
    }
}
