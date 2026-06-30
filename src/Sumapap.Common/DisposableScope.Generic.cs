using Sumapap.Common.Abstractions;

namespace Sumapap.Common
{
    public sealed class DisposableScope<T> : DisposableScope, IDisposableScope<T>
        where T : class
    {
        public T Context { get; }

        private DisposableScope(T context, Action action, Action disposingAction)
            : base(action, disposingAction)
        {
            Context = context;
        }

        public static DisposableScope<T> Create(T context, Action onDispose)
            => new(context, onDispose, () => { });

        public static DisposableScope<T> Create(T context, Action onDispose, Action onDisposing)
            => new(context, onDispose, onDisposing);
    }
}
