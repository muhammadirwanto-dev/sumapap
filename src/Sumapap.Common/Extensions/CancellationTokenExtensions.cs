namespace Sumapap.Common.Extensions
{
    public static class CancellationTokenExtensions
    {
        extension(CancellationToken token)
        {
            /// <summary>
            /// Registers a disposable scope to be disposed when the token is cancelled.
            /// </summary>
            public CancellationTokenRegistration DisposeOnCancel(IDisposable disposable)
            {
                return token.Register(d => { (d as IDisposable)?.Dispose(); }, disposable);
            }

            /// <summary>
            /// Creates a disposable scope tied to cancellation.
            /// </summary>
            public IDisposable CreateScope(Action onDispose)
            {
                var scope = DisposableScope.Create(onDispose);
                token.Register(scope.Dispose);

                return scope;
            }
        }
    }
}
