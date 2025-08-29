using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace WPADevTools.SadExtensions.UI.Layout
{
    public sealed class ControlListScroller : IDisposable
    {
        private readonly ControlsConsole _viewport;
        private readonly GameHost _host;
        private readonly int _stickyTopCount;

        private readonly Dictionary<ControlBase, (int x, int y, int h)> _baseLayout = new();
        private int _offset;
        private int _maxOffset;
        private int _lastWheel;
        private int _pinnedBottom;

        public ControlListScroller(ControlsConsole viewport, GameHost host, int stickyTopCount = 0)
        {
            _viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _stickyTopCount = Math.Max(0, stickyTopCount);

            _host.FrameUpdate += OnFrameUpdate;
            _lastWheel = _host.Mouse.ScrollWheelValue;
        }

        public void Dispose() => _host.FrameUpdate -= OnFrameUpdate;

        private void OnFrameUpdate(object? _, GameHost __)
        {
            if (!_viewport.IsVisible || !_viewport.IsEnabled || !_viewport.IsFocused)
                return;

            var wheel = _host.Mouse.ScrollWheelValue;
            var delta = wheel - _lastWheel;
            if (delta != 0)
            {
                var steps = delta / 120;
                if (steps == 0) steps = Math.Sign(delta);
                ScrollBy(-steps);
            }
            _lastWheel = wheel;
        }

        public void ScrollBy(int rows)
        {
            if (_maxOffset <= 0) return;
            var newOffset = Math.Clamp(_offset + rows, 0, _maxOffset);
            if (newOffset == _offset) return;
            _offset = newOffset;
            ApplyOffsets();
        }

        public void ResetToTop()
        {
            _offset = 0;
            ApplyOffsets();
        }

        public void Rebuild()
        {
            _baseLayout.Clear();

            foreach (var ctrl in _viewport.Controls)
            {
                int h = 1;
                try
                {
                    h = Math.Max(h, ctrl.Bounds.Height);
                } catch
                {
                    // Idk man don't be sad
                }

                _baseLayout[ctrl] = (ctrl.Position.X, ctrl.Position.Y, h);
            }

            _pinnedBottom = 0;
            int bottom = 0;
            int index = 0;

            foreach (var ctrl in _viewport.Controls)
            {
                var layout = _baseLayout[ctrl];
                if (index++ < _stickyTopCount)
                {
                    _pinnedBottom = Math.Max(_pinnedBottom, layout.y + layout.h);
                }
                else
                {
                    bottom = Math.Max(bottom, layout.y + layout.h);
                }
            }

            if (bottom == 0)
            {
                _maxOffset = 0;
                _offset = 0;
                ApplyOffsets();
                return;
            }

            int paddingBottom = 1;
            int scrollableHeight = Math.Max(0, (bottom - _pinnedBottom) + paddingBottom);
            int visibleHeight = Math.Max(0, _viewport.Height - _pinnedBottom);

            _maxOffset = Math.Max(0, scrollableHeight - visibleHeight);
            _offset = Math.Min(_offset, _maxOffset);

            ApplyOffsets();
        }

        private void ApplyOffsets()
        {
            int index = 0;
            foreach (var ctrl in _viewport.Controls)
            {
                var (x, y, h) = _baseLayout[ctrl];

                if (index++ < _stickyTopCount)
                {
                    ctrl.Position = new SadRogue.Primitives.Point(x, y);
                    ctrl.IsVisible = true;
                }
                else
                {
                    int newY = y - _offset;

                    // Cull anything above pinned area or below viewport
                    bool visible =
                        newY + h > _pinnedBottom &&
                        newY < _viewport.Height;

                    ctrl.IsVisible = visible;
                    if (visible)
                        ctrl.Position = new SadRogue.Primitives.Point(x, newY);
                }
            }
        }
    }
}
