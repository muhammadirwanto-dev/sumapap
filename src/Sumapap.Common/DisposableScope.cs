using Sumapap.Common.Abstractions;

namespace Sumapap.Common
{
    public class DisposableScope : IDisposableScope
    {
        public static readonly DisposableScope Empty = new(() => { }, () => { });

        private readonly Action _onDisposing;
        private readonly Action _onDispose;
        private bool _disposed;

        protected DisposableScope(Action onDispose, Action onDisposing)
        {
            _onDispose = onDispose;
            _onDisposing = onDisposing;
        }

        public static IDisposableScope Create(Action onDispose)
            => new DisposableScope(onDispose, () => { });

        public static IDisposableScope Create(Action onDispose, Action onDisposing)
            => new DisposableScope(onDispose, onDisposing);

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _onDisposing.Invoke();
            }

            _onDispose.Invoke();
            _disposed = true;
        }
    }
}
