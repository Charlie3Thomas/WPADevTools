using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using WPADevTools.Controller.State.AzureDevOps;
using WPADevTools.Messaging;

namespace WPADevTools.SadExtensions.UI.Panels
{
    public sealed class AdoEnvPickerPanel : FramedPanel
    {
        private readonly ControlsConsole _controls;
        private readonly int _innerW;
        private readonly int _innerH;

        private readonly ListBox _list;
        private readonly Button _btnRun;

        private AdoEnv? _selected;

        private sealed class EnvItem
        {
            public AdoEnv Env { get; }
            public EnvItem(AdoEnv env) => Env = env;
            public override string ToString() => Env.ToString();
        }

        public AdoEnvPickerPanel(int width, int height, Point position)
            : base(width, height, position)
        {
            _innerW = Surface.Width - 2;
            _innerH = Surface.Height - 2;

            _controls = new ControlsConsole(_innerW, _innerH)
            {
                Position = new Point(1, 1),
                UseMouse = true,
                UseKeyboard = true,
                FocusOnMouseClick = true
            };
            Surface.Children.Add(_controls);

            // Title
            _controls.Controls.Add(new Label("Environment:")
            {
                Position = new Point(1, 0),
                TextColor = Color.Yellow
            });

            // List (single-click selects by default)
            _list = new ListBox(_innerW - 2, _innerH - 4)
            {
                Position = new Point(1, 1),
                UseMouse = true,
                UseKeyboard = true
            };

            _list.Items.Add(new EnvItem(AdoEnv.Debug));
            _list.Items.Add(new EnvItem(AdoEnv.Develop));
            _list.Items.Add(new EnvItem(AdoEnv.Staging));
            _list.Items.Add(new EnvItem(AdoEnv.Production));

            // SINGLE-CLICK selection: react to selection *change*, not "execute"
            _list.SelectedItemChanged += (_, __) =>
            {
                _selected = (_list.SelectedItem as EnvItem)?.Env;
                _btnRun.IsEnabled = _selected.HasValue;

                if (_selected is AdoEnv env)
                {
                    //EventHub.Publish(AppMessage.InfoPanelUpdate("Environment:", GetEnvInfo(env)));
                }
                else
                {
                    //EventHub.Publish(AppMessage.InfoPanelClear());
                }
            };

            _controls.Controls.Add(_list);

            // Run button (acts on the current selection)
            _btnRun = new Button(12, 1)
            {
                Text = "Run",
                Position = new Point(_innerW - 13, _innerH - 2),
                IsEnabled = false
            };
            _btnRun.Click += (_, __) =>
            {
                if (_selected is AdoEnv env)
                    EventHub.Publish(AppMessage.AdoEnvChange(env));
            };
            _controls.Controls.Add(_btnRun);

            // Focus the list so single-click immediately works
            _controls.IsFocused = true;
            _list.IsFocused = true;
        }

        private static string GetEnvInfo(AdoEnv env) => env switch
        {
            AdoEnv.Debug => "Debug environment.\n• Local-only flags\n• Lowest quotas\n• Verbose logging",
            AdoEnv.Develop => "Develop environment.\n• Shared dev resources\n• Feature flags on by default",
            AdoEnv.Staging => "Staging environment.\n• Mirrors production\n• Final verification area",
            AdoEnv.Production => "Production environment.\n• Live traffic\n• Change windows apply",
            _ => ""
        };
    }
}
