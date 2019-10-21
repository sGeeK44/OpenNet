using System;

namespace OpenNet
{
    public abstract class DisposableBase : IDisposable
    {
        public bool IsDisposed { get; protected set; }

        public virtual void Dispose()
        {
            ReleaseManagedResources();
            ReleaseNativeResources();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }

        protected virtual void ReleaseManagedResources()
        {
        }
        protected virtual void ReleaseNativeResources()
        {
        }

        ~DisposableBase()
        {
            ReleaseNativeResources();
        }

    }
}
