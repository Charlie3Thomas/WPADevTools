// File: ScreenExtensions/TestScreen.cs
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.UI;              // Border
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;

namespace OingoBoingoConsole.ScreenExtensions
{
    public enum SplitLayout
    {
        None,
        Vertical,
        Horizontal
    }

    public readonly struct TestScreenOptions
    {
        public TestScreenOptions(
            SplitLayout layout = SplitLayout.Vertical,
            Color? panelA = null,
            Color? panelB = null,
            bool showBorders = false,
            bool toggleWithF6 = true,
            (int width, int height)? sizeCells = null)
        {
            Layout = layout;
            PanelAColor = panelA ?? new Color(18, 41, 82);
            PanelBColor = panelB ?? new Color(82, 18, 18);
            ShowBorders = showBorders;
            ToggleWithF6 = toggleWithF6;
            SizeCells = sizeCells;
        }

        public SplitLayout Layout { get; }
        public Color PanelAColor { get; }
        public Color PanelBColor { get; }
        public bool ShowBorders { get; }
        public bool ToggleWithF6 { get; }
        public (int width, int height)? SizeCells { get; }
    }

    public static class TestScreenFactory
    {
        public static TestScreen Create(TestScreenOptions options) => new TestScreen(options);

        public static TestScreen Create(Action<TestScreenOptions>? configure = null)
        {
            var opts = new TestScreenOptions();
            configure?.Invoke(opts);
            return new TestScreen(opts);
        }
    }

    /// <summary>
    /// Minimal starting screen with two ScreenSurfaces laid out 1:1,
    /// a transparent UI layer per panel, anchor demo tags, and samples
    /// of every built-in control placed via the Anchor helpers.
    /// </summary>
    public sealed class TestScreen : ScreenObject
    {
        private readonly ScreenSurface _panelA;
        private readonly ScreenSurface _panelB;

        private Rectangle _bounds;
        private SplitLayout _layout;
        private readonly Color _colorA;
        private readonly Color _colorB;
        private readonly bool _showBorders;

        // UI layers (controls) for each panel
        private readonly ControlsConsole _uiA;
        private readonly ControlsConsole _uiB;

        // Separate "tag" layers so we can rebuild anchor samples without nuking UI
        private readonly ScreenObject _tagsA;
        private readonly ScreenObject _tagsB;

        // --- Panel A controls (common controls) ---
        private readonly Button _btnOkA;
        private readonly Button _btnCancelA;
        private readonly CheckBox _chkA;
        private readonly ComboBox _cmbA;
        private readonly DrawingArea _drawA;
        private readonly Label _lblA;
        private readonly ListBox _listA;
        private readonly NumberBox _numA;
        private readonly ProgressBar _progA;
        private readonly RadioButton _radA;
        private readonly SelectionButton _selA;
        private readonly TextBox _txtA;
        private readonly ToggleSwitch _tgsA;

        // --- Panel B controls (specialized + remaining buttons) ---
        private readonly Button3d _btn3dB;
        private readonly ButtonBox _btnBoxB;
        private readonly Panel _panelCtlB;
        private readonly ScrollBar _scrollB;
        private readonly SurfaceViewer _viewerB;
        private readonly TabControl _tabsB;

        public TestScreen(TestScreenOptions options)
            : this(
                new Rectangle(
                    0, 0,
                    options.SizeCells?.width ?? AppSettings.CONSOLE_WIDTH,
                    options.SizeCells?.height ?? AppSettings.CONSOLE_HEIGHT),
                options)
        { }

        public TestScreen()
            : this(
                new Rectangle(0, 0, AppSettings.CONSOLE_WIDTH, AppSettings.CONSOLE_HEIGHT),
                new TestScreenOptions())
        { }

        public TestScreen(Rectangle bounds, TestScreenOptions options)
        {
            _bounds = bounds;
            _layout = options.Layout;
            _colorA = options.PanelAColor;
            _colorB = options.PanelBColor;
            _showBorders = options.ShowBorders;
            UseKeyboard = true;

            _panelA = new ScreenSurface(_bounds.Width, _bounds.Height);
            _panelB = new ScreenSurface(_bounds.Width, _bounds.Height);

            Children.Add(_panelA);
            Children.Add(_panelB);

            if (_showBorders)
            {
                Border.CreateForSurface(_panelA, "A");
                Border.CreateForSurface(_panelB, "B");
            }

            // --- Tag layers (for anchor demo labels) ---
            _tagsA = new ScreenObject();
            _tagsB = new ScreenObject();
            _panelA.Children.Add(_tagsA);
            _panelB.Children.Add(_tagsB);

            // --- UI layers (ControlsConsole) ---
            _uiA = new ControlsConsole(_bounds.Width, _bounds.Height) { Position = (0, 0) };
            _uiB = new ControlsConsole(_bounds.Width, _bounds.Height) { Position = (0, 0) };
            MakeTransparent(_uiA);
            MakeTransparent(_uiB);
            _panelA.Children.Add(_uiA);   // UI draws on top of tags
            _panelB.Children.Add(_uiB);

            // ===== Instantiate ALL controls we’ll demo =====

            // Panel A - "common controls"
            _btnOkA = new Button(10, 1) { Text = "OK" };
            _btnCancelA = new Button(10, 1) { Text = "Cancel" };
            _chkA = new CheckBox(Math.Max(10, "Check me".Length + 4), 1) { Text = "Check me", IsSelected = true };
            _cmbA = new ComboBox(16, 20, 6, new object[] { "Alpha", "Beta", "Gamma" }) { SelectedIndex = 0 };
            _drawA = new DrawingArea(20, 6);
            _lblA = new Label("Common Controls");
            _listA = new ListBox(20, 7);
            _listA.Items.Add("One"); _listA.Items.Add("Two"); _listA.Items.Add("Three");
            _numA = new NumberBox(8) { AllowDecimal = false, ShowUpDownButtons = true, NumberMinimum = 0, NumberMaximum = 100, Text = "42" };
            _progA = new ProgressBar(24, 1, HorizontalAlignment.Left) { Progress = 0.35f };
            _radA = new RadioButton(Math.Max(10, "Pick me".Length + 4), 1) { Text = "Pick me", IsSelected = true };
            _selA = new SelectionButton(14, 1) { Text = "Select…" };
            _txtA = new TextBox(18) { Text = "Edit me" };
            _tgsA = new ToggleSwitch(12, 1) { Text = "Toggle", IsSelected = false };

            _uiA.Controls.Add(_btnOkA);
            _uiA.Controls.Add(_btnCancelA);
            _uiA.Controls.Add(_chkA);
            _uiA.Controls.Add(_cmbA);
            _uiA.Controls.Add(_drawA);
            _uiA.Controls.Add(_lblA);
            _uiA.Controls.Add(_listA);
            _uiA.Controls.Add(_numA);
            _uiA.Controls.Add(_progA);
            _uiA.Controls.Add(_radA);
            _uiA.Controls.Add(_selA);
            _uiA.Controls.Add(_txtA);
            _uiA.Controls.Add(_tgsA);

            // Panel B - "specialized + remaining buttons"
            _btn3dB = new Button3d(12, 3) { Text = "3D Btn" };
            _btnBoxB = new ButtonBox(12, 3) { Text = "Box Btn" };
            _panelCtlB = new Panel(26, 8); // (no Title property)
            _scrollB = new ScrollBar(Orientation.Vertical, 8);

            // SurfaceViewer needs a surface to display; make a small sample
            var svSurface = new CellSurface(24, 6);
            svSurface.Fill(Color.White, new Color(25, 25, 25), 0, 0);
            svSurface.Print(2, 2, "SurfaceViewer");
            _viewerB = new SurfaceViewer(24, 6, svSurface);

            // TabControl requires TabItem(string header, Panel content)
            // We’ll keep the panels empty (no child adds) to avoid protected Controls access.
            var tabP1 = new Panel(18, 5);
            var tabP2 = new Panel(18, 5);
            var tabs = new[]
            {
                new TabItem("One", tabP1),
                new TabItem("Two", tabP2)
            };
            _tabsB = new TabControl(tabs, 20, 6);

            _uiB.Controls.Add(_btn3dB);
            _uiB.Controls.Add(_btnBoxB);
            _uiB.Controls.Add(_panelCtlB);
            _uiB.Controls.Add(_scrollB);
            _uiB.Controls.Add(_viewerB);
            _uiB.Controls.Add(_tabsB);

            ApplyLayout();

            if (options.ToggleWithF6)
                SadComponents.Add(new ToggleLayoutKeys(this));
        }

        private void ApplyLayout()
        {
            Rectangle a, b;

            if (_layout == SplitLayout.Vertical)
            {
                int leftW = Math.Max(1, _bounds.Width / 2);
                a = new Rectangle(_bounds.X, _bounds.Y, leftW, _bounds.Height);
                b = new Rectangle(_bounds.X + leftW, _bounds.Y, _bounds.Width - leftW, _bounds.Height);
            }
            else
            {
                int topH = Math.Max(1, _bounds.Height / 2);
                a = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, topH);
                b = new Rectangle(_bounds.X, _bounds.Y + topH, _bounds.Width, _bounds.Height - topH);
            }

            ResizeAndPlace(_panelA, a);
            ResizeAndPlace(_panelB, b);

            ResizeChildUi(_uiA, a.Size);
            ResizeChildUi(_uiB, b.Size);

            // --- Re-anchor all controls each pass so they stay glued ---

            // A: bottom corners buttons
            _btnOkA.AnchorTo(_uiA, Anchor.BottomLeft, (-1, -1));
            _btnCancelA.AnchorTo(_uiA, Anchor.BottomRight, (-1, -1));

            // A: top row
            _lblA.AnchorTo(_uiA, Anchor.TopLeft, (1, 1));
            _txtA.AnchorTo(_uiA, Anchor.TopMiddle, (0, 1));
            _cmbA.AnchorTo(_uiA, Anchor.TopRight, (-1, 1));

            // A: middle row (left/right)
            _listA.AnchorTo(_uiA, Anchor.MiddleLeft, (1, 0));
            _drawA.AnchorTo(_uiA, Anchor.MiddleRight, (-21, -3)); // center the 20x6 box roughly

            // A: lower row
            _chkA.AnchorTo(_uiA, Anchor.BottomLeft, (1 + _btnOkA.Width + 2, -1));  // next to OK
            _radA.AnchorTo(_uiA, Anchor.BottomLeft, (1 + _btnOkA.Width + _chkA.Width + 4, -1));
            _tgsA.AnchorTo(_uiA, Anchor.BottomLeft, (1 + _btnOkA.Width + _chkA.Width + _radA.Width + 6, -1));
            _numA.AnchorTo(_uiA, Anchor.BottomMiddle, (-20, -1));
            _progA.AnchorTo(_uiA, Anchor.BottomMiddle, (6, -1));
            _selA.AnchorTo(_uiA, Anchor.BottomMiddle, (-_selA.Width / 2, -3));

            // B: top & middle
            _btn3dB.AnchorTo(_uiB, Anchor.TopLeft, (1, 1));
            _btnBoxB.AnchorTo(_uiB, Anchor.TopRight, (-13, 1));
            _panelCtlB.AnchorTo(_uiB, Anchor.MiddleLeft, (1, -4));
            _viewerB.AnchorTo(_uiB, Anchor.MiddleRight, (-26, -3));

            // B: right edge scroll bar (use Height instead of non-existent Length)
            _scrollB.AnchorTo(_uiB, Anchor.MiddleRight, (-1, -_scrollB.Height / 2));

            // B: bottom tabs
            _tabsB.AnchorTo(_uiB, Anchor.BottomMiddle, (-_tabsB.Width / 2, -_tabsB.Height));

            // --- Paint backgrounds and headings ---
            _panelA.Surface.DefaultForeground = Color.White;
            _panelA.Surface.DefaultBackground = _colorA;
            _panelA.Surface.Clear();
            _panelA.Surface.Print(1, 1, _layout == SplitLayout.Vertical ? "Left panel" : "Top panel");

            _panelB.Surface.DefaultForeground = Color.White;
            _panelB.Surface.DefaultBackground = _colorB;
            _panelB.Surface.Clear();
            _panelB.Surface.Print(1, 1, _layout == SplitLayout.Vertical ? "Right panel" : "Bottom panel");

            PlaceAnchoredSamples();   // anchor labels (on separate tag layers)
        }

        private void PlaceAnchoredSamples()
        {
            _tagsA.Children.Clear();
            _tagsB.Children.Clear();

            void TagA(string text, Anchor anchor, Color bg, Color? fg = null, Point? offset = null)
            {
                var label = new Console(text.Length + 2, 1);
                label.Surface.DefaultBackground = bg;
                label.Surface.DefaultForeground = fg ?? Color.White;
                label.Surface.Clear();
                label.Print(1, 0, text);

                var area = new Rectangle(0, 0, _panelA.Surface.ViewWidth, _panelA.Surface.ViewHeight);
                label.AttachToAnchor(area, anchor, offset);

                _tagsA.Children.Add(label);
            }

            void TagB(string text, Anchor anchor, Color bg, Color? fg = null, Point? offset = null)
            {
                var label = new Console(text.Length + 2, 1);
                label.Surface.DefaultBackground = bg;
                label.Surface.DefaultForeground = fg ?? Color.White;
                label.Surface.Clear();
                label.Print(1, 0, text);

                var area = new Rectangle(0, 0, _panelB.Surface.ViewWidth, _panelB.Surface.ViewHeight);
                label.AttachToAnchor(area, anchor, offset);

                _tagsB.Children.Add(label);
            }

            // Panel A (blue)
            TagA("A:TopLeft", Anchor.TopLeft, _colorA.GetBrightest());
            TagA("A:TopRight", Anchor.TopRight, _colorA.GetBrightest());
            TagA("A:MiddleLeft", Anchor.MiddleLeft, _colorA.GetDarker());
            TagA("A:MiddleRight", Anchor.MiddleRight, _colorA.GetDarker());
            TagA("A:TopMiddle", Anchor.TopMiddle, _colorA.GetDarker());
            TagA("A:BottomMiddle", Anchor.BottomMiddle, _colorA.GetDarkest());
            TagA("A:BottomLeft", Anchor.BottomLeft, _colorA.GetDarkest());
            TagA("A:BottomRight", Anchor.BottomRight, _colorA.GetDarkest());
            TagA("A:Middle", Anchor.Middle, _colorA.GetDarkest());

            // Panel B (red-ish)
            TagB("B:TopLeft", Anchor.TopLeft, _colorB.GetBrightest());
            TagB("B:TopRight", Anchor.TopRight, _colorB.GetBrightest());
            TagB("B:MiddleLeft", Anchor.MiddleLeft, _colorB.GetDarker());
            TagB("B:MiddleRight", Anchor.MiddleRight, _colorB.GetDarker());
            TagB("B:TopMiddle", Anchor.TopMiddle, _colorB.GetDarker());
            TagB("B:BottomMiddle", Anchor.BottomMiddle, _colorB.GetDarkest());
            TagB("B:BottomLeft", Anchor.BottomLeft, _colorB.GetDarkest());
            TagB("B:BottomRight", Anchor.BottomRight, _colorB.GetDarkest());
            TagB("B:Middle", Anchor.Middle, _colorB.GetDarkest());
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
            ui.Position = (0, 0); // relative to its parent panel
            var cs = (CellSurface)ui.Surface;
            if (cs.ViewWidth != size.X || cs.ViewHeight != size.Y ||
                cs.Width != size.X || cs.Height != size.Y)
            {
                cs.Resize(size.X, size.Y, size.X, size.Y, clear: false);
            }

            // keep UI surface transparent after any resize
            MakeTransparent(ui);
        }

        private static void MakeTransparent(IScreenSurface surf)
        {
            surf.Surface.DefaultBackground = new Color(0, 0, 0, 0); // fully transparent
            surf.Surface.Clear();
        }

        private sealed class ToggleLayoutKeys : UpdateComponent
        {
            private readonly TestScreen _owner;
            public ToggleLayoutKeys(TestScreen owner) => _owner = owner;

            public override void Update(IScreenObject host, TimeSpan delta)
            {
                var k = GameHost.Instance.Keyboard;
                if (k.IsKeyPressed(Keys.F6))
                {
                    _owner._layout = _owner._layout == SplitLayout.Vertical
                        ? SplitLayout.Horizontal
                        : SplitLayout.Vertical;

                    _owner.ApplyLayout();
                }
            }
        }
    }
}
