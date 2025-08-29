using SadConsole;
using SadConsole.UI;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Layout
{
    public sealed class VerticalScrollHelper : IDisposable
    {
        private readonly ControlsConsole _viewport;
        private readonly GameHost _host;
        private int _maxScroll;
        private int _current;

        public VerticalScrollHelper(ControlsConsole viewport, GameHost host)
        {
            _viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));
            _host = host ?? throw new ArgumentNullException(nameof(host));

            _host.FrameUpdate += OnFrameUpdate;
        }

        private void OnFrameUpdate(object? sender, GameHost e)
        {
            if (!_viewport.IsVisible || !_viewport.IsEnabled || !_viewport.IsFocused)
            {
                return;
            }

            int delta = _host.Mouse.ScrollWheelValueChange;
            if (delta == 0)
            {
                return;
            }

            int steps = delta / 120;
            if (steps == 0)
            {
                steps = Math.Sign(delta);
            }

            _current = Math.Clamp(_current - steps, 0, _maxScroll);

            var v = _viewport.Surface.View;
            if (v.Y != _current)
            {
                _viewport.Surface.View = new Rectangle(v.X, _current, v.Width, v.Height);
            }
        }

        public void RecalculateContentHeight(int paddingBottom = 1)
        {
            int contentBottom = 0;

            foreach (var ctrl in _viewport.Controls)
            {
                int h = 1;
                try
                {
                    h = Math.Max(h, ctrl.Bounds.Height);
                }
                catch
                {
                    // Don't be sad. This code isn't real.
                }

                contentBottom = Math.Max(contentBottom, ctrl.Position.Y + h);
            }

            int contentHeight = contentBottom + paddingBottom;
            _maxScroll = Math.Max(0, contentHeight - _viewport.Height);
            _current = Math.Clamp(_current, 0, _maxScroll);

            var v = _viewport.Surface.View;
            _viewport.Surface.View = new Rectangle(v.X, _current, v.Width, v.Height);
        }

        public void ResetToTop()
        {
            _current = 0;
            var v = _viewport.Surface.View;
            _viewport.Surface.View = new Rectangle(v.X, 0, v.Width, v.Height);
        }

        public void Dispose() => _host.FrameUpdate -= OnFrameUpdate;
    }
}
