using SadRogue.Primitives;
using WPADevTools.Controller.State.Config;
using WPADevTools.Controller.State.MainMenu;
using WPADevTools.Controller.State.MainMenu.Components;
using WPADevTools.Messaging;

namespace WPADevTools.Controller.State.Implementations.MainMenu
{
    public sealed class MainMenuState
        : ComposedStateBase
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
            (next switch
            {
                Branch.Menu => (Action)ShowMenuBranch,
                Branch.ExampleTask => (Action)ShowExampleTaskBranch,
                _ => Noop
            })();
        }

        private void ShowMenuBranch()
        {
            ContentHost.Clear();

            ShowMenu("Main Menu", focus: true);
            ResetMenuScrollTop();

            _taskCts?.Cancel();
            _taskCts?.Dispose();

            _taskCts = null;
            _example = null;
        }

        private void ShowExampleTaskBranch()
        {
            ContentHost.Clear();

            HideMenu();
            SetHeader("Example Task");

            _example = ContentHost.Add(new ExamplePanel(84, 20, new Point(3, 4)));
            _example.ShowWorking();

            _taskCts?.Cancel();
            _taskCts = new CancellationTokenSource();
            _ = _example.RunAsync(TimeSpan.FromSeconds(2), _taskCts.Token);
        }

        private static void Noop() { }
    }
}
