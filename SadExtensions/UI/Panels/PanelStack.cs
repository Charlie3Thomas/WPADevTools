using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Panels
{
    /// <summary>Simple vertical stack layout helper for panels.</summary>
    public sealed class PanelStack
    {
        private readonly PanelHost _host;
        private readonly Point _origin;
        private readonly int _gapY;
        private int _count;

        public PanelStack(PanelHost host, Point origin, int gapY = 1)
        {
            _host = host;
            _origin = origin;
            _gapY = gapY;
        }

        public T Add<T>(T panel, int? fixedHeight = null) where T : PanelViewBase
        {
            var y = _origin.Y + _count;
            panel.Surface.Position = new Point(_origin.X, y);
            _host.Add(panel);
            _count += fixedHeight ?? panel.Surface.Height + _gapY;
            return panel;
        }

        public void Reset() => _count = 0;
    }
}
