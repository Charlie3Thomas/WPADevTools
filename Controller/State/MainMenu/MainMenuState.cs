using SadRogue.Primitives;
using WPADevTools.Controller.State.Configuration;
using WPADevTools.Controller.State.MainMenu;
using WPADevTools.Controller.State.MainMenu.Components;
using WPADevTools.Messaging;
using WPADevTools.SadExtensions.UI.Buttons;

namespace WPADevTools.Controller.State.Implementations.MainMenu
{
    public sealed class MainMenuState : ComposedStateBase
    {
        private CancellationTokenSource? _taskCts;
        private ExamplePanel? _example;

        public MainMenuState()
        : base(
            name: Branch.Menu.ToString(),
            spec: StateConfigs.MainMenu.Spec,
            chromeSize: StateConfigs.MainMenu.ChromeSize)
        {
        }

        public override Task OnEnterAsync(Controller controller, CancellationToken ct)
        {
            var t = base.OnEnterAsync(controller, ct);

            controller.BranchChanged += OnBranchChanged;
            OnBranchChanged(controller.Branch);

            return t;
        }

        public override Task OnExitAsync(Controller controller, CancellationToken ct)
        {
            controller.BranchChanged -= OnBranchChanged;

            _taskCts?.Cancel();
            _taskCts?.Dispose();
            _taskCts = null;
            _example = null;

            return base.OnExitAsync(controller, ct);
        }

        protected override void HandleAppMessage(AppMessage msg)
        {
            // Let base handle common toasts; nothing custom here right now.
        }

        private void OnBranchChanged(Branch next)
        {
            // Swap content based on the model (Controller.Branch)
            ContentHost.Clear();

            switch (next)
            {
                case Branch.Menu:
                    MenuUi.IsVisible = true;
                    MenuUi.IsEnabled = true;
                    MenuUi.IsFocused = true;

                    SetHeader("Main Menu");
                    FocusFirstButton();
                    break;

                case Branch.ExampleTask:
                    MenuUi.IsVisible = false;
                    MenuUi.IsEnabled = false;

                    SetHeader("Example Task");

                    _example = ContentHost.Add(new ExamplePanel(84, 20, new Point(3, 4)));
                    _example.ShowWorking();

                    _taskCts?.Cancel();
                    _taskCts = new CancellationTokenSource();
                    _ = _example.RunAsync(TimeSpan.FromSeconds(2), _taskCts.Token);
                    break;
            }
        }
    }
}
