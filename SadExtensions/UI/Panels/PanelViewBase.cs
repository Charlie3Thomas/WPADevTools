using SadConsole;
using SadRogue.Primitives;
using WPADevTools.SadExtensions.UI.Layout;

namespace WPADevTools.SadExtensions.UI.Panels
{
    public abstract class PanelViewBase : IDisposable
    {
        public ScreenSurface Surface { get; }
        public ScreenObject Root => Surface;

        private bool _attached;
        private bool _disposed;

        protected PanelViewBase(int width, int height, Point position)
        {
            Surface = new ScreenSurface(width, height) { Position = position };
        }

        public void CenterTo(ScreenSurface parent, int offsetX = 0, int offsetY = 0)
            => Surface.Position = Anchors.Center(parent, Surface.Width, Surface.Height, offsetX, offsetY);

        public void AttachTo(ScreenObject parent)
        {
            if (_disposed || _attached) return;
            parent.Children.Add(Surface);
            _attached = true;
            OnAttached(parent);
        }

        public void DetachFrom(ScreenObject parent)
        {
            if (_disposed || !_attached) return;
            parent.Children.Remove(Surface);
            _attached = false;
            OnDetached(parent);
        }

        protected virtual void OnAttached(ScreenObject parent) { }
        protected virtual void OnDetached(ScreenObject parent) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Surface.Parent?.Children.Remove(Surface);
                _attached = false;

                Surface.Dispose();
            }

            _disposed = true;
        }
    }
}
