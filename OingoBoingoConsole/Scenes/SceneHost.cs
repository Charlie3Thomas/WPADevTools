// ===== File: Scenes/SceneHost.cs =====
using SadConsole;
using SadConsole.Components;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace OingoBoingoConsole.Scenes
{
    public sealed class SceneHost : ScreenObject
    {
        private readonly Console _title;
        private readonly ScreenObject _stage;
        private readonly ScreenObject _overlay;

        private readonly List<SceneDefinition> _defsInOrder;
        private readonly int _pickerBuffer = 1;

        private ControlsConsole _picker;

        private ListBox _pickerList;
        private SceneKey _currentKey;

        private Border? _pickerBorder;

        public SceneRouter Router { get; }

        public SceneHost(Dictionary<SceneKey, SceneDefinition> defs, SceneKey initialKey)
        {
            _title = new Console(AppSettings.CONSOLE_WIDTH, 1) { Position = (0, 0) };
            _title.Surface.DefaultBackground = Color.Black;
            _title.Surface.DefaultForeground = Color.Yellow;
            _title.Surface.Clear();
            Children.Add(_title);

            _stage = new ScreenObject { Position = (0, 1) };
            Children.Add(_stage);

            Router = new SceneRouter(defs, _stage);
            _defsInOrder = defs.Values.ToList();

            Router.SceneChanged += (scene, def) =>
            {
                _currentKey = def.Key;
                _title.Surface.Clear();
                _title.Surface.Print(1, 0,
                    $"{def.Title}  [{def.Key}]  —  F1:Scenes");
                scene.Position = (0, 0);

                FitPickerToContent(); // align picker to middle panel
            };

            _overlay = new ScreenObject { Position = (0, 0) };
            _stage.Children.Add(_overlay);

            _picker = new ControlsConsole(28, Math.Max(10, AppSettings.CONSOLE_HEIGHT - 2))
            {
                Position = (0, 0),
                IsVisible = false,
                IsEnabled = false
            };

            //Border.CreateForSurface(_picker, "Scenes");
            EnsurePickerBorder();
            _overlay.Children.Add(_picker);

            _pickerList = new ListBox(_picker.Width - 2, _picker.Height - 4) { Position = (1, 2) };
            foreach (var d in _defsInOrder)
                _pickerList.Items.Add((d.Key, d.Title));

            _pickerList.SelectedItemExecuted += (_, e) =>
            {
                if (e.Item is ValueTuple<SceneKey, string> sel)
                {
                    HidePicker();
                    Router.NavigateTo(sel.Item1, fade: true);
                }
            };
            _picker.Controls.Add(_pickerList);

            DrawPickerCaption();
            Children.Add(_picker);

            Router.NavigateTo(initialKey, replace: false, fade: false);

            SadComponents.Add(new HostHotkeys(this, Router, _defsInOrder));
        }

        private void DrawPickerCaption()
        {
            if (_picker?.Surface is SadConsole.CellSurface s)
            {
                // row 1 is inside the border, row 0 would collide with the border line/title
                _picker.Surface.Print(1, 1,
                    "Enter = open".CreateColored(Colors.CreateSadConsoleBlue().Title, null));
                s.IsDirty = true; // ensure it shows up this frame
            }
        }

        private void EnsurePickerBorder()
        {
            // Remove old border if present
            if (_pickerBorder != null)
            {
                _pickerBorder.Parent?.Children.Remove(_pickerBorder);
                _pickerBorder = null;
            }

            // Create a plain border (NO SHADOW) so it never bleeds into the footer
            var bp = Border.BorderParameters
                         .GetDefault()
                         .AddTitle("Scenes"); // no .AddShadow()

            _pickerBorder = new Border(_picker, bp);
        }

        private void TogglePicker() { if (_picker.IsVisible) HidePicker(); else ShowPicker(); }
        private void ShowPicker()
        {
            FitPickerToContent(); 
            _picker.IsVisible = true;
            _picker.IsEnabled = true;
            _picker.IsFocused = true;
        }

        private void HidePicker() { _picker.IsVisible = _picker.IsEnabled = false; if (Router.Current != null) Router.Current.IsFocused = true; }

        private void FitPickerToContent()
        {
            if (Router.Current == null) return;

            var content = Router.Current.ContentAreaRect;

            // Outer (visible) height = content minus symmetric buffer
            int outerTop = _stage.Position.Y + content.Y + _pickerBuffer;
            int outerHeight = Math.Max(6, content.Height - (_pickerBuffer * 2));

            // Inner console height = outer minus 2 (top/bottom border lines)
            int pickerHeight = Math.Max(4, outerHeight - 2);

            // Only rebuild if something changed
            var cs = (SadConsole.CellSurface)_picker.Surface;
            bool needsRebuild = cs.ViewHeight != pickerHeight || _picker.Position.Y != outerTop + 1;

            if (needsRebuild)
            {
                RecreatePicker(pickerHeight, outerTop);
            }
            else
            {
                EnsurePickerBorder();
                DrawPickerCaption();
            }
                
        }

        private void RecreatePicker(int pickerHeight, int outerTop)
        {
            // Capture items and selection
            var items = _pickerList?.Items?.Cast<object>().ToList() ?? new List<object>();
            int selected = _pickerList?.SelectedIndex ?? -1;
            bool wasVisible = _picker?.IsVisible ?? false;
            bool wasEnabled = _picker?.IsEnabled ?? false;

            // Remove existing border and console
            if (_pickerBorder != null)
            {
                _pickerBorder.Parent?.Children.Remove(_pickerBorder);
                _pickerBorder = null;
            }
            if (_picker != null)
                Children.Remove(_picker);

            // Build new console at requested height (inner console height)
            var picker = new ControlsConsole(_picker?.Width ?? 28, pickerHeight)
            {
                Position = (0, outerTop + 1),   // +1 because border sits at (-1,-1)
                IsVisible = wasVisible,
                IsEnabled = wasEnabled
            };
            picker.Surface.DefaultBackground = Color.Black;
            picker.Surface.DefaultForeground = Color.White;
            picker.Surface.Clear();

            // List size: inside a 1px border top/bottom + title row = 4 rows overhead
            int listHeight = Math.Max(2, pickerHeight - 4);   // ListBox scrollbar needs >= 2
            var list = new ListBox(picker.Width - 2, listHeight) { Position = (1, 2) };
            foreach (var it in items) list.Items.Add(it);
            if (selected >= 0 && selected < list.Items.Count) list.SelectedIndex = selected;

            // Wire click
            list.SelectedItemExecuted += (_, e) =>
            {
                if (e.Item is ValueTuple<SceneKey, string> sel)
                {
                    HidePicker();
                    Router.NavigateTo(sel.Item1, fade: true);
                }
            };

            picker.Controls.Add(list);

            // Add to host, update fields, and restore border
            Children.Add(picker);
            _picker = picker;
            _pickerList = list;

            EnsurePickerBorder();
            DrawPickerCaption();
        }


        private sealed class HostHotkeys : UpdateComponent
        {
            private readonly SceneHost _host;
            private readonly SceneRouter _router;
            private readonly List<SceneDefinition> _defs;

            public HostHotkeys(SceneHost host, SceneRouter router, List<SceneDefinition> defs)
            { _host = host; _router = router; _defs = defs; }

            public override void Update(IScreenObject _, TimeSpan __)
            {
                var k = GameHost.Instance.Keyboard;

                if (k.IsKeyPressed(SadConsole.Input.Keys.F1)) _host.TogglePicker();

                if ((k.IsKeyDown(SadConsole.Input.Keys.LeftAlt) || k.IsKeyDown(SadConsole.Input.Keys.RightAlt)) &&
                    k.IsKeyPressed(SadConsole.Input.Keys.Left)) _router.Back();

                if ((k.IsKeyDown(SadConsole.Input.Keys.LeftAlt) || k.IsKeyDown(SadConsole.Input.Keys.RightAlt)) &&
                    k.IsKeyPressed(SadConsole.Input.Keys.Right)) _router.Forward();

                if (k.IsKeyPressed(SadConsole.Input.Keys.N)) JumpRelative(+1);
                if (k.IsKeyPressed(SadConsole.Input.Keys.P)) JumpRelative(-1);

                for (int i = 0; i < Math.Min(9, _defs.Count); i++)
                {
                    var key = (SadConsole.Input.Keys)((int)SadConsole.Input.Keys.D1 + i);
                    if (k.IsKeyPressed(key)) { _router.NavigateTo(_defs[i].Key, fade: true); break; }
                }
            }

            private void JumpRelative(int delta)
            {
                if (_defs.Count == 0) return;

                var idx = _defs.FindIndex(d => d.Key == _host._currentKey);
                if (idx < 0) idx = 0;
                idx = (idx + delta + _defs.Count) % _defs.Count;

                _router.NavigateTo(_defs[idx].Key, fade: true);
            }
        }
    }
}
