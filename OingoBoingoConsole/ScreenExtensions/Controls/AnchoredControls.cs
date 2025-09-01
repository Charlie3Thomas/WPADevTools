using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace OingoBoingoConsole.ScreenExtensions
{
    /// <summary>
    /// Anchor helpers for built-in SadConsole controls.
    /// Requires your AnchorLayout + ControlAnchors (AnchorTo) extensions.
    /// </summary>
    public static class AnchoredControls
    {
        // -------- Common Controls --------

        public static Button AddAnchoredButton(
            this ControlsConsole host,
            string text,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = -1,
            int height = 1,
            bool clampInside = true)
        {
            var w = width <= 0 ? Math.Max(6, (text?.Length ?? 0) + 2) : width;
            var ctrl = new Button(w, height) { Text = text ?? string.Empty };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static Button3d AddAnchoredButton3d(
            this ControlsConsole host,
            string text,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = 11,
            int height = 3,
            bool clampInside = true)
        {
            var ctrl = new Button3d(width, height) { Text = text ?? string.Empty };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static ButtonBox AddAnchoredButtonBox(
            this ControlsConsole host,
            string text,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = 12,
            int height = 3,
            bool clampInside = true)
        {
            var ctrl = new ButtonBox(width, height) { Text = text ?? string.Empty };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static CheckBox AddAnchoredCheckBox(
            this ControlsConsole host,
            string text,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = -1,
            int height = 1,
            bool isChecked = false,
            bool clampInside = true)
        {
            var w = width <= 0 ? Math.Max(8, (text?.Length ?? 0) + 4) : width;
            var ctrl = new CheckBox(w, height) { Text = text ?? string.Empty, IsSelected = isChecked };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static ComboBox AddAnchoredComboBox(
            this ControlsConsole host,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = 16,
            int dropdownWidth = 20,
            int dropdownHeight = 6,
            IEnumerable<object>? items = null,
            int selectedIndex = -1,
            bool clampInside = true)
        {
            var seed = (items ?? Array.Empty<object>()).ToArray();
            var ctrl = new ComboBox(width, dropdownWidth, dropdownHeight, seed);
            host.Controls.Add(ctrl);

            if (selectedIndex >= 0 && selectedIndex < seed.Length)
                ctrl.SelectedIndex = selectedIndex;

            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static DrawingArea AddAnchoredDrawingArea(
            this ControlsConsole host,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = 20,
            int height = 6,
            bool clampInside = true)
        {
            var ctrl = new DrawingArea(width, height);
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static Label AddAnchoredLabel(
            this ControlsConsole host,
            string text,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool clampInside = true)
        {
            var ctrl = new Label(text ?? string.Empty);
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static ListBox AddAnchoredListBox(
            this ControlsConsole host,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = 20,
            int height = 7,
            IEnumerable<string>? items = null,
            bool clampInside = true)
        {
            var ctrl = new ListBox(width, height);
            if (items != null)
            {
                foreach (var it in items)
                    ctrl.Items.Add(it);
            }
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static NumberBox AddAnchoredNumberBox(
            this ControlsConsole host,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = 8,
            double value = 0,
            double min = 0,
            double max = 100,
            bool allowDecimal = false,
            bool showUpDown = true,
            bool clampInside = true)
        {
            var ctrl = new NumberBox(width)
            {
                AllowDecimal = allowDecimal,
                ShowUpDownButtons = showUpDown,
                NumberMinimum = (long)min,
                NumberMaximum = (long)max
            };

            ctrl.Text = allowDecimal ? value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                                     : ((long)value).ToString();

            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static ProgressBar AddAnchoredProgressBar(
            this ControlsConsole host,
            int width,
            Anchor anchor,
            (int x, int y)? offset = null,
            HorizontalAlignment alignment = HorizontalAlignment.Left,
            bool clampInside = true,
            float initialProgress = 0f)
        {
            var ctrl = new ProgressBar(width, 1, alignment)
            {
                Progress = Math.Clamp(initialProgress, 0f, 1f)
            };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static RadioButton AddAnchoredRadioButton(
            this ControlsConsole host,
            string text,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = -1,
            int height = 1,
            bool isChecked = false,
            bool clampInside = true)
        {
            var w = width <= 0 ? Math.Max(8, (text?.Length ?? 0) + 4) : width;
            var ctrl = new RadioButton(w, height) { Text = text ?? string.Empty, IsSelected = isChecked };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static SelectionButton AddAnchoredSelectionButton(
            this ControlsConsole host,
            string text,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = -1,
            int height = 1,
            bool clampInside = true)
        {
            var w = width <= 0 ? Math.Max(10, (text?.Length ?? 0) + 2) : width;
            var ctrl = new SelectionButton(w, height) { Text = text ?? string.Empty };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static TextBox AddAnchoredTextBox(
            this ControlsConsole host,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = 16,
            string? text = null,
            bool clampInside = true)
        {
            var ctrl = new TextBox(width) { Text = text ?? string.Empty };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static ToggleSwitch AddAnchoredToggleSwitch(
            this ControlsConsole host,
            string text,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = -1,
            int height = 1,
            bool isOn = false,
            bool clampInside = true)
        {
            var w = width <= 0 ? Math.Max(10, (text?.Length ?? 0) + 6) : width;
            var ctrl = new ToggleSwitch(w, height) { Text = text ?? string.Empty, IsSelected = isOn };
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        // -------- Specialized Controls --------

        public static Panel AddAnchoredPanel(
            this ControlsConsole host,
            Anchor anchor,
            (int x, int y)? offset = null,
            int width = 24,
            int height = 8,
            bool clampInside = true)
        {
            var ctrl = new Panel(width, height);
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static ScrollBar AddAnchoredScrollBar(
            this ControlsConsole host,
            Anchor anchor,
            (int x, int y)? offset = null,
            Orientation orientation = Orientation.Vertical,
            int length = 8,
            bool clampInside = true)
        {
            var ctrl = new ScrollBar(orientation, length);
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static SurfaceViewer AddAnchoredSurfaceViewer(
            this ControlsConsole host,
            int width,
            int height,
            ICellSurface? surface,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool clampInside = true)
        {
            var ctrl = new SurfaceViewer(width, height, surface);
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static TabControl AddAnchoredTabControl(
            this ControlsConsole host,
            IEnumerable<TabItem> tabItems,
            int width,
            int height,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool clampInside = true)
        {
            var ctrl = new TabControl(tabItems, width, height);
            host.Controls.Add(ctrl);
            ctrl.AnchorTo(host, anchor, offset ?? (0, 0), clampInside);
            return ctrl;
        }

        public static TabControl AddAnchoredTabControl(
            this ControlsConsole host,
            int width,
            int height,
            Anchor anchor,
            (int x, int y)? offset = null,
            bool clampInside = true,
            params TabItem[] tabItems)
        {
            return host.AddAnchoredTabControl(
                (IEnumerable<TabItem>)tabItems,
                width, height, anchor, offset, clampInside);
        }
    }
}
