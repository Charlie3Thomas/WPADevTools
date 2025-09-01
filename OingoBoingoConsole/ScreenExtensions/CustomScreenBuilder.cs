using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace OingoBoingoConsole.ScreenExtensions
{
    public sealed class CustomScreenBuilder
    {
        private CustomScreenOptions _opts = new();
        private readonly List<AnchoredControlSpec> _specs = [];
        private readonly List<AnchoredObjectSpec> _objSpecs = [];

        // ---------- options ----------
        public CustomScreenBuilder WithLayout(SplitLayout layout)
        {
            _opts = new CustomScreenOptions(layout, _opts.PanelAColor, _opts.PanelBColor, _opts.ShowBorders, _opts.ToggleWithF6,
                _opts.SizeCells, _opts.HeaderHeight, _opts.FooterHeight, _opts.HeaderColor, _opts.FooterColor);
            return this;
        }

        public CustomScreenBuilder WithSize(int width, int height)
        {
            _opts = new CustomScreenOptions(_opts.Layout, _opts.PanelAColor, _opts.PanelBColor, _opts.ShowBorders, _opts.ToggleWithF6,
                (width, height), _opts.HeaderHeight, _opts.FooterHeight, _opts.HeaderColor, _opts.FooterColor);
            return this;
        }

        public CustomScreenBuilder WithPanelColors(Color a, Color b)
        {
            _opts = new CustomScreenOptions(_opts.Layout, a, b, _opts.ShowBorders, _opts.ToggleWithF6,
                _opts.SizeCells, _opts.HeaderHeight, _opts.FooterHeight, _opts.HeaderColor, _opts.FooterColor);
            return this;
        }

        public CustomScreenBuilder WithBorders(bool show = true)
        {
            _opts = new CustomScreenOptions(_opts.Layout, _opts.PanelAColor, _opts.PanelBColor, show, _opts.ToggleWithF6,
                _opts.SizeCells, _opts.HeaderHeight, _opts.FooterHeight, _opts.HeaderColor, _opts.FooterColor);
            return this;
        }

        public CustomScreenBuilder ToggleWithF6(bool on = true)
        {
            _opts = new CustomScreenOptions(_opts.Layout, _opts.PanelAColor, _opts.PanelBColor, _opts.ShowBorders, on,
                _opts.SizeCells, _opts.HeaderHeight, _opts.FooterHeight, _opts.HeaderColor, _opts.FooterColor);
            return this;
        }

        // NEW: header/footer configuration
        public CustomScreenBuilder WithHeaderFooterHeights(int headerHeight = 3, int footerHeight = 3)
        {
            _opts = new CustomScreenOptions(_opts.Layout, _opts.PanelAColor, _opts.PanelBColor, _opts.ShowBorders, _opts.ToggleWithF6,
                _opts.SizeCells, headerHeight, footerHeight, _opts.HeaderColor, _opts.FooterColor);
            return this;
        }

        public CustomScreenBuilder WithHeaderColor(Color color)
        {
            _opts = new CustomScreenOptions(_opts.Layout, _opts.PanelAColor, _opts.PanelBColor, _opts.ShowBorders, _opts.ToggleWithF6,
                _opts.SizeCells, _opts.HeaderHeight, _opts.FooterHeight, color, _opts.FooterColor);
            return this;
        }

        public CustomScreenBuilder WithFooterColor(Color color)
        {
            _opts = new CustomScreenOptions(_opts.Layout, _opts.PanelAColor, _opts.PanelBColor, _opts.ShowBorders, _opts.ToggleWithF6,
                _opts.SizeCells, _opts.HeaderHeight, _opts.FooterHeight, _opts.HeaderColor, color);
            return this;
        }

        // ---------- generic add ----------
        public CustomScreenBuilder AddControl(
            PanelTarget panel,
            Func<ControlsConsole, ControlBase> factory,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool clampInside = true,
            bool keepAnchoredOnResize = true)
        {
            _specs.Add(new AnchoredControlSpec(panel, factory, anchor, offset, clampInside, keepAnchoredOnResize));
            return this;
        }

        // ---------- helpers for built-ins ----------
        public CustomScreenBuilder AddButton(PanelTarget p, string text, Anchor a, (int x, int y)? off = null, int width = -1, int height = 1)
            => AddControl(p, host => {
                var w = width <= 0 ? Math.Max(6, (text?.Length ?? 0) + 2) : width;
                return new Button(w, height) { Text = text ?? string.Empty };
            }, a, off);

        public CustomScreenBuilder AddLabel(PanelTarget p, string text, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new Label(text ?? string.Empty), a, off);

        public CustomScreenBuilder AddTextBox(PanelTarget p, int width, string? text, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new TextBox(width) { Text = text ?? string.Empty }, a, off);

        public CustomScreenBuilder AddComboBox(
            PanelTarget p,
            int width,
            int ddWidth,
            int ddHeight,
            IEnumerable<object> items,
            int selectedIndex,
            Anchor a,
            (int x, int y)? off = null)
            => AddControl(p, _ =>
            {
                var seed = (items ?? Array.Empty<object>()).ToArray();
                var c = new ComboBox(width, ddWidth, ddHeight, seed);
                if (selectedIndex >= 0 && selectedIndex < seed.Length)
                    c.SelectedIndex = selectedIndex;
                return c;
            }, a, off);

        public CustomScreenBuilder AddCheckBox(PanelTarget p, string text, bool isChecked, Anchor a, (int x, int y)? off = null, int width = -1)
            => AddControl(p, _ => {
                var w = width <= 0 ? Math.Max(8, (text?.Length ?? 0) + 4) : width;
                return new CheckBox(w, 1) { Text = text ?? string.Empty, IsSelected = isChecked };
            }, a, off);

        public CustomScreenBuilder AddRadioButton(PanelTarget p, string text, bool isChecked, Anchor a, (int x, int y)? off = null, int width = -1)
            => AddControl(p, _ => {
                var w = width <= 0 ? Math.Max(8, (text?.Length ?? 0) + 4) : width;
                return new RadioButton(w, 1) { Text = text ?? string.Empty, IsSelected = isChecked };
            }, a, off);

        public CustomScreenBuilder AddToggleSwitch(PanelTarget p, string text, bool isOn, Anchor a, (int x, int y)? off = null, int width = -1)
            => AddControl(p, _ => {
                var w = width <= 0 ? Math.Max(10, (text?.Length ?? 0) + 6) : width;
                return new ToggleSwitch(w, 1) { Text = text ?? string.Empty, IsSelected = isOn };
            }, a, off);

        public CustomScreenBuilder AddSelectionButton(PanelTarget p, string text, Anchor a, (int x, int y)? off = null, int width = -1)
            => AddControl(p, _ => {
                var w = width <= 0 ? Math.Max(10, (text?.Length ?? 0) + 2) : width;
                return new SelectionButton(w, 1) { Text = text ?? string.Empty };
            }, a, off);

        public CustomScreenBuilder AddListBox(PanelTarget p, int width, int height, IEnumerable<string> items, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => {
                var lb = new ListBox(width, height);
                if (items != null) foreach (var it in items) lb.Items.Add(it);
                return lb;
            }, a, off);

        public CustomScreenBuilder AddDrawingArea(PanelTarget p, int width, int height, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new DrawingArea(width, height), a, off);

        public CustomScreenBuilder AddNumberBox(PanelTarget p, int width, double value, double min, double max, bool allowDecimal, bool showUpDown, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => {
                var nb = new NumberBox(width)
                {
                    AllowDecimal = allowDecimal,
                    ShowUpDownButtons = showUpDown,
                    NumberMinimum = (long)min,
                    NumberMaximum = (long)max,
                };
                nb.Text = allowDecimal ? value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                                       : ((long)value).ToString();
                return nb;
            }, a, off);

        public CustomScreenBuilder AddProgressBar(PanelTarget p, int width, float progress, HorizontalAlignment align, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new ProgressBar(width, 1, align) { Progress = Math.Clamp(progress, 0f, 1f) }, a, off);

        public CustomScreenBuilder AddButton3d(PanelTarget p, string text, int width, int height, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new Button3d(width, height) { Text = text ?? string.Empty }, a, off);

        public CustomScreenBuilder AddButtonBox(PanelTarget p, string text, int width, int height, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new ButtonBox(width, height) { Text = text ?? string.Empty }, a, off);

        public CustomScreenBuilder AddPanel(PanelTarget p, int width, int height, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new Panel(width, height), a, off);

        public CustomScreenBuilder AddScrollBar(PanelTarget p, Orientation orientation, int length, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new ScrollBar(orientation, length), a, off);

        public CustomScreenBuilder AddSurfaceViewer(PanelTarget p, int width, int height, ICellSurface? surface, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new SurfaceViewer(width, height, surface), a, off);

        public CustomScreenBuilder AddTabControl(PanelTarget p, int width, int height, IEnumerable<TabItem> tabs, Anchor a, (int x, int y)? off = null)
            => AddControl(p, _ => new TabControl(tabs ?? Enumerable.Empty<TabItem>(), width, height), a, off);

        // ---------- build ----------
        public CustomScreen Build()
            => CustomScreenFactory.Create(new CustomScreenDefinition(_opts, _specs));

        public CustomScreenBuilder AddSurfaceObject(
            PanelTarget panel,
            Func<IScreenObject> factory,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool clampInside = true,
            bool autoSizeToPanel = true)
        {
            _objSpecs.Add(new AnchoredObjectSpec(panel, factory, anchor, offset, clampInside, autoSizeToPanel));
            return this;
        }

    }
}
