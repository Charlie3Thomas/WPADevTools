using SadConsole;
using SadConsole.Components;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace OingoBoingoConsole.ScreenExtensions
{
    /// <summary>
    /// Extensions to position SadConsole UI controls using our Anchor helpers.
    /// Works for any ControlBase (Button, Label, TextBox, etc).
    /// </summary>
    public static class ControlAnchorExtensions
    {
        /// <summary>
        /// Anchor a control inside a host ControlsConsole. Optionally keep it anchored when the host resizes.
        /// If the control isn't already in the host, it will be added.
        /// </summary>
        public static T AnchorTo<T>(
            this T ctrl,
            ControlsConsole host,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool keepAnchoredOnResize = true,
            bool clampInside = true)
        where T : ControlBase
        {
            if (ctrl.Parent != host)
                host.Controls.Add(ctrl);

            // initial placement
            Place(ctrl, host, anchor, offset ?? (0, 0), clampInside);

            // bind to keep anchored
            if (keepAnchoredOnResize)
            {
                var binder = host.GetSadComponent<AnchorBinder>();
                if (binder is null)
                {
                    binder = new AnchorBinder(host);
                    host.SadComponents.Add(binder);
                }

                binder.Set(ctrl, anchor, offset ?? (0, 0), clampInside);
            }

            return ctrl;
        }

        /// <summary>
        /// Convenience overload that infers the host from ctrl.Parent (must already be added to a ControlsConsole).
        /// </summary>
        public static T AnchorTo<T>(
            this T ctrl,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool keepAnchoredOnResize = true,
            bool clampInside = true) where T : ControlBase
        {
            if (ctrl.Parent is not ControlsConsole host)
                throw new InvalidOperationException("AnchorTo without host requires the control to already be added to a ControlsConsole.");

            return AnchorTo(ctrl, host, anchor, offset, keepAnchoredOnResize, clampInside);
        }

        // Sugar for common anchors
        public static T TopLeft<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.TopLeft, off, keep);

        public static T TopRight<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.TopRight, off, keep);

        public static T BottomLeft<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.BottomLeft, off, keep);

        public static T BottomRight<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.BottomRight, off, keep);

        public static T TopMiddle<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.TopMiddle, off, keep);

        public static T BottomMiddle<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.BottomMiddle, off, keep);

        public static T MiddleLeft<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.MiddleLeft, off, keep);

        public static T MiddleRight<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.MiddleRight, off, keep);

        public static T Middle<T>(this T c, ControlsConsole h, (int x, int y)? off = null, bool keep = true) where T : ControlBase
            => c.AnchorTo(h, Anchor.Middle, off, keep);

        // ---------- internals ----------

        private static void Place(ControlBase ctrl, ControlsConsole host, Anchor anchor, (int x, int y) offset, bool clampInside)
        {
            var area = new Rectangle(0, 0, host.Width, host.Height);
            var size = (ctrl.Width, ctrl.Height);

            // Use your existing Anchor helper
            Point pos = AnchorLayout.GetPosition(area, size, anchor, offset);

            if (clampInside)
            {
                int maxX = Math.Max(0, area.Width - size.Item1);
                int maxY = Math.Max(0, area.Height - size.Item2);
                if (pos.X < area.X) pos = new Point(area.X, pos.Y);
                if (pos.Y < area.Y) pos = new Point(pos.X, area.Y);
                if (pos.X > maxX) pos = new Point(maxX, pos.Y);
                if (pos.Y > maxY) pos = new Point(pos.X, maxY);
            }

            ctrl.Position = pos;
        }

        /// <summary>
        /// Keeps a set of controls anchored to a ControlsConsole when it resizes.
        /// </summary>
        private sealed class AnchorBinder : UpdateComponent
        {
            private readonly ControlsConsole _host;
            private readonly Dictionary<ControlBase, (Anchor anchor, (int x, int y) offset, bool clamp)> _map = new();
            private (int w, int h) _lastSize;

            public AnchorBinder(ControlsConsole host)
            {
                _host = host;
                _lastSize = (host.Width, host.Height);
            }

            public void Set(ControlBase ctrl, Anchor anchor, (int x, int y) offset, bool clamp)
                => _map[ctrl] = (anchor, offset, clamp);

            public override void Update(IScreenObject host, TimeSpan delta)
            {
                // Host size changed?
                if (_lastSize.w != _host.Width || _lastSize.h != _host.Height)
                {
                    _lastSize = (_host.Width, _host.Height);
                    ReanchorAll();
                }
                else
                {
                    // If any anchored control changed size, honor that too.
                    ReanchorAll(checkSizeOnly: true);
                }
            }

            private readonly Dictionary<ControlBase, (int w, int h)> _sizesCache = new();

            private void ReanchorAll(bool checkSizeOnly = false)
            {
                List<ControlBase> toRemove = null;

                foreach (var kv in _map)
                {
                    var ctrl = kv.Key;
                    if (ctrl.Parent != _host)
                    {
                        (toRemove ??= new()).Add(ctrl);
                        continue;
                    }

                    var size = (ctrl.Width, ctrl.Height);
                    if (checkSizeOnly)
                    {
                        if (!_sizesCache.TryGetValue(ctrl, out var prev) || prev.w != size.Item1 || prev.h != size.Item2)
                            Place(ctrl, _host, kv.Value.anchor, kv.Value.offset, kv.Value.clamp);
                    }
                    else
                    {
                        Place(ctrl, _host, kv.Value.anchor, kv.Value.offset, kv.Value.clamp);
                    }

                    _sizesCache[ctrl] = size;
                }

                if (toRemove != null)
                    foreach (var c in toRemove) _map.Remove(c);
            }
        }
    }
}
