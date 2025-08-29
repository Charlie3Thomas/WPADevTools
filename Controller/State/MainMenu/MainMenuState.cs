using System;
using System.Threading;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using WPADevTools.Messaging;
using WPADevTools.Controller.State.MainMenu;
using WPADevTools.SadExtensions.UI.Adornments;
using WPADevTools.SadExtensions.Common.Components;
using WPADevTools.SadExtensions.UI.Labels;
using WPADevTools.SadExtensions.UI.Buttons;
using WPADevTools.SadExtensions.UI.Panels;
using WPADevTools.Controller.State.MainMenu.Components;

namespace WPADevTools.Controller.State.Implementations.MainMenu
{
    public sealed class MainMenuState : AppStateBase
    {
        private readonly ChromeSurface _chrome;
        private readonly MainMenuConsole _menuUi;

        private readonly PanelHost _panelHost;   // scene panels
        private readonly PanelHost _toastHost;   // overlay toasts (multiple)

        private Button? _btnExample;
        private Button? _btnQuit;

        private CancellationTokenSource? _taskCts;
        private ExamplePanel? _example;

        public MainMenuState() : base(Branch.Menu.ToString())
        {
            _chrome = new ChromeSurface(new Dimensions(90, 30));
            Root.Children.Add(_chrome);

            _menuUi = new MainMenuConsole();
            _chrome.Children.Add(_menuUi);

            _panelHost = new PanelHost(PanelHostMode.Single);
            _chrome.Children.Add(_panelHost);

            _toastHost = new PanelHost(PanelHostMode.Multiple);
            _chrome.Children.Add(_toastHost); // added last -> renders on top

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
            EventHub.Message += OnAppMessage;    // listen for QuitRequested/Quit/ToastDismiss

            OnBranchChanged(controller.Branch);
            return Task.CompletedTask;
        }

        public override Task OnExitAsync(Controller controller, CancellationToken ct)
        {
            controller.BranchChanged -= OnBranchChanged;
            EventHub.Message -= OnAppMessage;

            // Clean up overlays on exit
            _toastHost.Clear();
            _panelHost.Clear();
            DisposeExample();
            return Task.CompletedTask;
        }

        public override void Update(TimeSpan delta)
        {
            // No keyboard enum usage here; toast has buttons
        }

        private void OnBranchChanged(Branch branch) => SwitchBranch(branch);

        private void OnAppMessage(AppMessage msg)
        {
            switch (msg.Type)
            {
                case AppMessageType.QuitRequested:
                    ShowQuitToast();
                    break;

                case AppMessageType.ToastDismiss:
                    _toastHost.Clear();
                    break;

                case AppMessageType.Quit:
                    // Controller will actually exit the app. We just clear UI overlays.
                    _toastHost.Clear();
                    break;
            }
        }

        private void ShowQuitToast()
        {
            var toast = new ConfirmToast(new Point(0, 0), "Really quit?");
            toast.CenterTo(_chrome);
            _toastHost.Clear();
            _toastHost.Add(toast);
        }


        private void BuildMenu()
        {
            _menuUi.Controls.Clear();
            _menuUi.FocusOnMouseClick = true;

            var title = new TitleLabel("WPA Dev Tools");
            _menuUi.Controls.Add(title);

            var stack = new ButtonStack(_menuUi, x: 1, startY: 3, gapY: 2);

            _btnExample = new ExampleButton(stack);
            _btnQuit = new QuitButton(stack);

            var hintY = stack.NextY + 1;
            var hint = new HintLabel("Enter/Click to select • ESC: Back") { Position = new Point(0, hintY) };
            _menuUi.Controls.Add(hint);

            _menuUi.IsFocused = true;
            _btnExample.IsFocused = true;
            _menuUi.Controls.FocusedControl = _btnExample;
        }

        private void SwitchBranch(Branch next)
        {
            DisposeExample();
            _panelHost.Clear();

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

                    ChromeHeaderBox.WriteText(_chrome, "Main Menu");
                    break;

                case Branch.ExampleTask:
                    _menuUi.IsVisible = false;
                    _menuUi.IsEnabled = false;

                    ChromeHeaderBox.WriteText(_chrome, "Example Task");

                    _example = _panelHost.Add(new ExamplePanel(84, 20, new Point(3, 4)));
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
            _example = null; // host clears the visuals
        }
    }
}
