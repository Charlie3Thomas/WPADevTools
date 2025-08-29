using SadConsole;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using WPADevTools.Messaging;
using WPADevTools.SadExtensions.Common.Buttons;
using WPADevTools.Controller.State.MainMenu;
using WPADevTools.Controller.State.MainMenu.Components;
using WPADevTools.SadExtensions.UI.Adornments;
using WPADevTools.SadExtensions.Common.Components;
using WPADevTools.SadExtensions.UI.Labels;

namespace WPADevTools.Controller.State.Implementations.MainMenu
{
    public sealed class MainMenuState
        : AppStateBase
    {
        private readonly ChromeSurface _chrome;
        private readonly MainMenuConsole _menuUi;

        private Button? _btnExample;
        private Button? _btnQuit;

        private CancellationTokenSource? _taskCts;
        private ExamplePanel? _example;

        public MainMenuState()
            : base(Branch.Menu.ToString())
        {
            _chrome = new ChromeSurface(new Dimensions(90, 30));
            Root.Children.Add(_chrome);

            _menuUi = new MainMenuConsole();
            _chrome.Children.Add(_menuUi);

            BuildMenu();
            new ChromeHeaderBox("Main Menu").Draw(_chrome);
            new ChromeFooterBox("ESC: Back • Mouse/Keys enabled").Draw(_chrome);
        }

        public override Task OnEnterAsync(Controller controller, CancellationToken ct)
        {
            _menuUi.IsVisible = true;
            _menuUi.IsEnabled = true;
            _menuUi.IsFocused = true;

            controller.BranchChanged += OnBranchChanged;

            OnBranchChanged(controller.Branch);

            return Task.CompletedTask;
        }

        public override Task OnExitAsync(Controller controller, CancellationToken ct)
        {
            controller.BranchChanged -= OnBranchChanged;
            return Task.CompletedTask;
        }

        public override void Update(TimeSpan delta)
        {
            var host = Controller.Instance.Host;
            if (host != null &&
                Controller.Instance.Branch != Branch.Menu &&
                host.Keyboard.IsKeyReleased(SadConsole.Input.Keys.Escape))
            {
                EventHub.Publish(AppMessage.BranchChange(Branch.Menu));
            }
        }

        private void OnBranchChanged(Branch branch) => SwitchBranch(branch);

        private void BuildMenu()
        {
            _menuUi.Controls.Clear();
            _menuUi.FocusOnMouseClick = true;

            var title = new TitleLabel("WPA Dev Tools");

            _menuUi.Controls.Add(title);

            _btnExample = new ExampleButton();
            _menuUi.Controls.Add(_btnExample);

            _btnQuit = new QuitButton();
            _menuUi.Controls.Add(_btnQuit);

            var hint = new HintLabel("Enter/Click to select • ESC: Back");

            _menuUi.Controls.Add(hint);

            _menuUi.IsFocused = true;
            _btnExample.IsFocused = true;
        }

        private void SwitchBranch(Branch next)
        {
            DisposeExample();

            switch (next)
            {
                case Branch.Menu:
                    _menuUi.IsVisible = true;
                    _menuUi.IsEnabled = true;
                    _menuUi.IsFocused = true;

                    if (_btnExample != null)
                    {
                        _menuUi.Controls.FocusedControl = _btnExample;
                        _btnExample.IsFocused = true;
                    }

                    _chrome.Surface.Print(2, 1, "Main Menu".PadRight(Math.Max(0, _chrome.Width - 4)), Color.Yellow);
                    break;

                case Branch.ExampleTask:
                    _menuUi.IsVisible = false;
                    _menuUi.IsEnabled = false;

                    ChromeHeaderBox.WriteText(_chrome, "Example Task");

                    _example = new ExamplePanel(84, 20, new Point(3, 4));
                    _example.AttachTo(_chrome);
                    _example.ShowWorking();

                    _taskCts?.Cancel();
                    _taskCts = new CancellationTokenSource();
                    _ = _example.RunAsync(TimeSpan.FromSeconds(2), _taskCts.Token);
                    break;
            }
        }

        private void DisposeExample()
        {
            _taskCts?.Cancel();
            _taskCts?.Dispose();
            _taskCts = null;

            if (_example != null)
            {
                _example.DetachFrom(_chrome);
                _example.Dispose();
                _example = null;
            }
        }
    }
}
