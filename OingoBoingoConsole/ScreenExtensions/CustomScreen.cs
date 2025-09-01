using OingoBoingoConsole.Scenes;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace OingoBoingoConsole.ScreenExtensions
{
    public enum PanelTarget
    {
        Header,     // <- new
        A,
        B,
        Footer      // <- new
    }

    public sealed record AnchoredObj(
        IScreenObject Obj,
        PanelTarget Panel,
        Anchor Anchor,
        (int x, int y) Offset,
        bool ClampInside,
        bool AutoSizeToPanel
    );

    public readonly struct CustomScreenOptions
    {
        public CustomScreenOptions(
            SplitLayout layout = SplitLayout.Vertical,
            Color? panelA = null,
            Color? panelB = null,
            bool showBorders = false,
            bool toggleWithF6 = true,
            (int width, int height)? sizeCells = null,
            int headerHeight = 3,
            int footerHeight = 3,
            Color? headerColor = null,
            Color? footerColor = null)
        {
            Layout = layout;
            PanelAColor = panelA ?? new Color(18, 41, 82);
            PanelBColor = panelB ?? new Color(82, 18, 18);
            ShowBorders = showBorders;
            ToggleWithF6 = toggleWithF6;
            SizeCells = sizeCells;
            HeaderHeight = Math.Max(1, headerHeight);
            FooterHeight = Math.Max(1, footerHeight);
            HeaderColor = headerColor ?? Color.Black;
            FooterColor = footerColor ?? Color.Black;
        }

        public SplitLayout Layout { get; }
        public Color PanelAColor { get; }
        public Color PanelBColor { get; }
        public bool ShowBorders { get; }
        public bool ToggleWithF6 { get; }
        public (int width, int height)? SizeCells { get; }
        public int HeaderHeight { get; }
        public int FooterHeight { get; }
        public Color HeaderColor { get; }
        public Color FooterColor { get; }
    }

    public readonly struct AnchoredObjectSpec
    {
        public AnchoredObjectSpec(
            PanelTarget panel,
            Func<IScreenObject> factory,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool clampInside = true,
            bool autoSizeToPanel = true)
        {
            Panel = panel;
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Anchor = anchor;
            Offset = offset ?? (0, 0);
            ClampInside = clampInside;
            AutoSizeToPanel = autoSizeToPanel;
        }

        public PanelTarget Panel { get; }
        public Func<IScreenObject> Factory { get; }
        public Anchor Anchor { get; }
        public (int x, int y) Offset { get; }
        public bool ClampInside { get; }
        public bool AutoSizeToPanel { get; }
    }

    public readonly struct AnchoredControlSpec
    {
        public AnchoredControlSpec(
            PanelTarget panel,
            Func<ControlsConsole, ControlBase> factory,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool clampInside = true,
            bool keepAnchoredOnResize = true)
        {
            Panel = panel;
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Anchor = anchor;
            Offset = offset ?? (0, 0);
            ClampInside = clampInside;
            KeepAnchoredOnResize = keepAnchoredOnResize;
        }

        public PanelTarget Panel { get; }
        public Func<ControlsConsole, ControlBase> Factory { get; }
        public Anchor Anchor { get; }
        public (int x, int y) Offset { get; }
        public bool ClampInside { get; }
        public bool KeepAnchoredOnResize { get; }
    }

    public readonly struct CustomScreenDefinition
    {
        public CustomScreenDefinition(
            CustomScreenOptions options,
            IEnumerable<AnchoredControlSpec> controls,
            IEnumerable<AnchoredObjectSpec>? objects = null)
        {
            Options = options;
            Controls = controls?.ToArray() ?? Array.Empty<AnchoredControlSpec>();
            Objects = objects?.ToArray() ?? Array.Empty<AnchoredObjectSpec>();
        }

        public CustomScreenOptions Options { get; }
        public IReadOnlyList<AnchoredControlSpec> Controls { get; }
        public IReadOnlyList<AnchoredObjectSpec> Objects { get; }
    }

    public static class CustomScreenFactory
    {
        public static CustomScreen Create(CustomScreenDefinition def) => new(def);
    }

    public sealed class CustomScreen : ScreenObject
    {
        private readonly CustomScreenOptions _opts;
        private readonly IReadOnlyList<AnchoredControlSpec> _specs;

        // header/footer surfaces
        private readonly ScreenSurface _header;
        private readonly ScreenSurface _footer;
        private readonly ControlsConsole _uiHeader;
        private readonly ControlsConsole _uiFooter;

        // content panels
        private readonly ScreenSurface _panelA;
        private readonly ScreenSurface _panelB;
        private readonly ControlsConsole _uiA;
        private readonly ControlsConsole _uiB;

        private Rectangle _bounds;
        private Rectangle _headerRect, _footerRect, _contentRect;
        private SplitLayout _layout;

        private readonly List<AnchoredObj> _anchoredObjs = [];

        private readonly IReadOnlyList<AnchoredObjectSpec> _objSpecs = [];


        public Rectangle HeaderRect => _headerRect;
        public Rectangle FooterRect => _footerRect;
        public Rectangle ContentAreaRect => _contentRect; // full mid panel (may include border)
        public Rectangle ContentInnerRect  // safe inner region for overlays/pickers
            => _opts.ShowBorders
                ? new Rectangle(_contentRect.X, _contentRect.Y + 1,
                                _contentRect.Width, Math.Max(0, _contentRect.Height - 2))
                : _contentRect;

        public bool HasBorders => _opts.ShowBorders;
        //public Rectangle ContentAreaRect { get; private set; }

        public CustomScreen(CustomScreenDefinition def)
        {
            _opts = def.Options;
            _specs = def.Controls;

            _bounds = new Rectangle(
                0, 0,
                _opts.SizeCells?.width ?? AppSettings.CONSOLE_WIDTH,
                _opts.SizeCells?.height ?? AppSettings.CONSOLE_HEIGHT);

            _layout = _opts.Layout;
            UseKeyboard = true;

            // ---- header/footer
            _header = new ScreenSurface(_bounds.Width, Math.Max(1, _opts.HeaderHeight));
            _footer = new ScreenSurface(_bounds.Width, Math.Max(1, _opts.FooterHeight));
            Children.Add(_header);
            Children.Add(_footer);

            _uiHeader = new ControlsConsole(_bounds.Width, _header.Surface.ViewHeight) { Position = (0, 0) };
            _uiFooter = new ControlsConsole(_bounds.Width, _footer.Surface.ViewHeight) { Position = (0, 0) };
            MakeTransparent(_uiHeader);
            MakeTransparent(_uiFooter);
            _header.Children.Add(_uiHeader);
            _footer.Children.Add(_uiFooter);

            // ---- content panels
            _panelA = new ScreenSurface(_bounds.Width, _bounds.Height);
            _panelB = new ScreenSurface(_bounds.Width, _bounds.Height);
            Children.Add(_panelA);
            Children.Add(_panelB);

            _uiA = new ControlsConsole(_bounds.Width, _bounds.Height) { Position = (0, 0) };
            _uiB = new ControlsConsole(_bounds.Width, _bounds.Height) { Position = (0, 0) };
            MakeTransparent(_uiA);
            MakeTransparent(_uiB);
            _panelA.Children.Add(_uiA);
            _panelB.Children.Add(_uiB);

            // borders (once)
            if (_opts.ShowBorders)
            {
                Border.CreateForSurface(_header, "Header");
                Border.CreateForSurface(_panelA, "A");
                Border.CreateForSurface(_panelB, "B");
                Border.CreateForSurface(_footer, "Footer");
            }

            // build controls into the correct host console
            BuildControlsFromSpec();

            _objSpecs = def.Objects;

            ApplyLayout();

            if (_opts.ToggleWithF6)
                SadComponents.Add(new ToggleLayoutKeys(this));
        }

        public ControlsConsole GetControlsHost(PanelTarget p) => p switch
        {
            PanelTarget.Header => _uiHeader,
            PanelTarget.Footer => _uiFooter,
            PanelTarget.A => _uiA,
            PanelTarget.B => _uiB,
            _ => _uiA
        };

        private void BuildObjectsFromSpec()
        {
            foreach (var spec in _objSpecs)
            {
                var obj = spec.Factory();
                AddSurfaceObject(spec.Panel, obj, spec.Anchor, spec.Offset, spec.ClampInside, spec.AutoSizeToPanel);
            }
        }

        private void BuildControlsFromSpec()
        {
            foreach (var spec in _specs)
            {
                ControlsConsole host = spec.Panel switch
                {
                    PanelTarget.Header => _uiHeader,
                    PanelTarget.Footer => _uiFooter,
                    PanelTarget.A => _uiA,
                    PanelTarget.B => _uiB,
                    _ => _uiA
                };

                var ctrl = spec.Factory(host);
                if (ctrl.Parent != host) host.Controls.Add(ctrl);
                ctrl.AnchorTo(host, spec.Anchor, spec.Offset, spec.KeepAnchoredOnResize, spec.ClampInside);
            }
        }

        private void ApplyLayout()
        {
            // 1) Clamp header/footer in CELLS
            int hh = Math.Max(0, _opts.HeaderHeight);
            int fh = Math.Max(0, _opts.FooterHeight);

            // 2) Compute rects (all in CELLS)
            var headerRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, hh);
            var footerRect = new Rectangle(_bounds.X, _bounds.Y + _bounds.Height - fh, _bounds.Width, fh);
            var contentRect = new Rectangle(_bounds.X, _bounds.Y + hh, _bounds.Width, Math.Max(0, _bounds.Height - hh - fh));

            _headerRect = headerRect;
            _footerRect = footerRect;
            _contentRect = contentRect;

            _contentRect = new Rectangle(
                _bounds.X,
                _bounds.Y + hh,
                _bounds.Width,
                Math.Max(0, _bounds.Height - hh - fh));

            int midY = headerRect.Y + headerRect.Height;
            int midH = Math.Max(0, _bounds.Height - hh - fh);
            var middleRect = new Rectangle(_bounds.X, midY, _bounds.Width, midH);

            // 3) Resize/place header/footer
            ResizeAndPlace(_header, headerRect);
            ResizeChildUi(_uiHeader, headerRect.Size);
            Paint(_header, Color.White, _opts.HeaderColor);

            ResizeAndPlace(_footer, footerRect);
            ResizeChildUi(_uiFooter, footerRect.Size);
            Paint(_footer, Color.White, _opts.FooterColor);

            // 4) Split the MIDDLE ONLY
            Rectangle a, b;
            switch (_layout)
            {
                case SplitLayout.Vertical:
                    int leftW = Math.Max(1, _contentRect.Width / 2);
                    a = new Rectangle(_contentRect.X, _contentRect.Y, leftW, _contentRect.Height);
                    b = new Rectangle(_contentRect.X + leftW, _contentRect.Y,
                                      _contentRect.Width - leftW, _contentRect.Height);
                    break;

                case SplitLayout.Horizontal:
                    int topH = Math.Max(1, _contentRect.Height / 2);
                    a = new Rectangle(_contentRect.X, _contentRect.Y, _contentRect.Width, topH);
                    b = new Rectangle(_contentRect.X, _contentRect.Y + topH,
                                      _contentRect.Width, _contentRect.Height - topH);
                    break;

                case SplitLayout.None:
                default:
                    a = _contentRect;
                    b = new Rectangle(_contentRect.X, _contentRect.Y, 0, 0); // unused
                    break;
            }

            // 5) Panels
            ResizeAndPlace(_panelA, a);
            ResizeChildUi(_uiA, a.Size);
            Paint(_panelA, Color.White, _opts.PanelAColor);

            if (b.Width > 0 && b.Height > 0)
            {
                _panelB.IsVisible = true;
                ResizeAndPlace(_panelB, b);
                ResizeChildUi(_uiB, b.Size);
                Paint(_panelB, Color.White, _opts.PanelBColor);
            }
            else
            {
                _panelB.IsVisible = false;
            }

            foreach (var it in _anchoredObjs)
            {
                AnchorObject(it);
            }
        }

        private static void Paint(IScreenSurface s, Color fg, Color bg)
        {
            s.Surface.DefaultForeground = fg;
            s.Surface.DefaultBackground = bg;
            s.Surface.Clear();
        }

        private static void ResizeAndPlace(IScreenSurface surf, in Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                surf.IsVisible = false;
                return;
            }

            surf.IsVisible = true;
            surf.Position = rect.Position;

            var cs = (CellSurface)surf.Surface;
            if (cs.ViewWidth != rect.Width || cs.ViewHeight != rect.Height ||
                cs.Width != rect.Width || cs.Height != rect.Height)
            {
                cs.Resize(rect.Width, rect.Height, rect.Width, rect.Height, clear: false);
            }
        }

        private static void ResizeChildUi(ControlsConsole ui, in Point size)
        {
            ui.Position = (0, 0);
            var cs = (CellSurface)ui.Surface;
            if (cs.ViewWidth != size.X || cs.ViewHeight != size.Y ||
                cs.Width != size.X || cs.Height != size.Y)
            {
                cs.Resize(size.X, size.Y, size.X, size.Y, clear: false);
            }
            MakeTransparent(ui);
        }

        private static void MakeTransparent(IScreenSurface surf)
        {
            surf.Surface.DefaultBackground = new Color(0, 0, 0, 0);
            surf.Surface.Clear();
        }

        // Back button belongs in the header now
        public void AddSceneBackButton(SceneKey parent, SceneRouter router)
        {
            var back = new Button(8, 1) { Text = "< Back" };
            _uiHeader.Controls.Add(back);
            back.AnchorTo(_uiHeader, Anchor.TopLeft, (1, 1));
            back.Click += (_, __) => router.NavigateTo(parent, fade: true);
        }

        private sealed class ToggleLayoutKeys : UpdateComponent
        {
            private readonly CustomScreen _owner;
            public ToggleLayoutKeys(CustomScreen owner) => _owner = owner;

            public override void Update(IScreenObject host, TimeSpan delta)
            {
                var k = GameHost.Instance.Keyboard;
                if (k.IsKeyPressed(Keys.F6))
                {
                    _owner._layout = _owner._layout == SplitLayout.Vertical
                        ? SplitLayout.Horizontal
                        : (_owner._layout == SplitLayout.Horizontal ? SplitLayout.None : SplitLayout.Vertical);
                    _owner.ApplyLayout();
                }
            }
        }

        private Rectangle GetPanelLocalArea(PanelTarget p) => p switch
        {
            PanelTarget.Header => new Rectangle(0, 0, _header.Surface.ViewWidth, _header.Surface.ViewHeight),
            PanelTarget.Footer => new Rectangle(0, 0, _footer.Surface.ViewWidth, _footer.Surface.ViewHeight),
            PanelTarget.A => new Rectangle(0, 0, _panelA.Surface.ViewWidth, _panelA.Surface.ViewHeight),
            PanelTarget.B => new Rectangle(0, 0, _panelB.Surface.ViewWidth, _panelB.Surface.ViewHeight),
            _ => new Rectangle(0, 0, 1, 1)
        };

        private IScreenObject GetPanelContainer(PanelTarget p) => p switch
        {
            PanelTarget.Header => _header,
            PanelTarget.Footer => _footer,
            PanelTarget.A => _panelA,
            PanelTarget.B => _panelB,
            _ => _panelA
        };

        public T AddSurfaceObject<T>(
            PanelTarget panel,
            T obj,
            Anchor anchor = Anchor.Middle,
            (int x, int y)? offset = null,
            bool clampInside = true,
            bool autoSizeToPanel = true) where T : IScreenObject
        {
            var host = GetPanelContainer(panel);
            host.Children.Add(obj);

            var spec = new AnchoredObj(obj, panel, anchor, offset ?? (0, 0), clampInside, autoSizeToPanel);
            _anchoredObjs.Add(spec);

            AnchorObject(spec); // initial placement
            return obj;
        }

        private void AnchorObject(AnchoredObj it)
        {
            var area = GetPanelLocalArea(it.Panel);

            // Optional auto-resize to panel
            if (it.AutoSizeToPanel && it.Obj is IScreenSurface ss)
            {
                var cs = (CellSurface)ss.Surface;
                if (cs.ViewWidth != area.Width || cs.ViewHeight != area.Height ||
                    cs.Width != area.Width || cs.Height != area.Height)
                    cs.Resize(area.Width, area.Height, area.Width, area.Height, clear: false);

                // Keep DonutPanel's internal buffers in sync
                if (it.Obj is DonutPanel dp)
                    dp.ResizeTo(area.Width, area.Height);
            }

            // Determine the object's size
            (int w, int h) size = it.Obj is IScreenSurface s
                ? (s.Surface.ViewWidth, s.Surface.ViewHeight)
                : it.Obj is ControlsConsole cc
                    ? (cc.Width, cc.Height)
                    : (1, 1);

            // Local (0,0,w,h) anchoring inside the panel
            var pos = AnchorLayout.GetPosition(area, size, it.Anchor, it.Offset);

            if (it.ClampInside)
            {
                int maxX = Math.Max(area.X, area.X + area.Width - size.w);
                int maxY = Math.Max(area.Y, area.Y + area.Height - size.h);
                if (pos.X < area.X) pos = new Point(area.X, pos.Y);
                if (pos.Y < area.Y) pos = new Point(pos.X, area.Y);
                if (pos.X > maxX) pos = new Point(maxX, pos.Y);
                if (pos.Y > maxY) pos = new Point(pos.X, maxY);
            }

            it.Obj.Position = pos;
        }
    }
}
