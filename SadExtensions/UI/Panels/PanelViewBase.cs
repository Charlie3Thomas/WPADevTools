using System;
using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Panels
{
    /// <summary>Base class for a UI panel backed by a ScreenSurface.</summary>
    public abstract class PanelViewBase : IDisposable
    {
        public ScreenSurface Surface { get; }
        public ScreenObject Root => Surface;

        private bool _attached;

        protected PanelViewBase(int width, int height, Point position)
        {
            Surface = new ScreenSurface(width, height) { Position = position };
        }

        public void AttachTo(ScreenObject parent)
        {
            if (_attached) return;
            parent.Children.Add(Surface);
            _attached = true;
            OnAttached(parent);
        }

        public void DetachFrom(ScreenObject parent)
        {
            if (!_attached) return;
            parent.Children.Remove(Surface);
            _attached = false;
            OnDetached(parent);
        }

        protected virtual void OnAttached(ScreenObject parent) { }
        protected virtual void OnDetached(ScreenObject parent) { }

        public virtual void Dispose() => Surface.Dispose();
    }
}
