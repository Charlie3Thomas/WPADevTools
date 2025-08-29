using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.Controller.State.Implementations.MainMenu
{
    public sealed partial class MainMenuState
    {
        private SadConsole.ScreenSurface? _examplePanel;
        private CancellationTokenSource? _taskCts;
        private WPADevTools.SadExtensions.ExamplePanel? _example;

        public override void Update(TimeSpan delta)
        {
            var host = Controller.Instance.Host;
            if (host != null &&
                _active != Branch.Menu &&
                host.Keyboard.IsKeyReleased(SadConsole.Input.Keys.Escape))
            {
                SwitchBranch(Branch.Menu);
            }
        }

        private void SwitchBranch(Branch next)
        {
            _active = next;
            RemoveBranchPanels();

            switch (_active)
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

                    _chrome.Surface.Print(2, 1, "Main Menu".PadRight(8), Color.Yellow);
                    break;

                case Branch.ExampleTask:
                    _menuUi.IsVisible = false;
                    _menuUi.IsEnabled = false;

                    {
                        var header = "Example Task";
                        var width = _chrome.Width - 4;
                        _chrome.Surface.Print(2, 1, header.PadRight(Math.Max(0, width)), Color.Yellow);
                    }

                    _example?.Dispose();
                    _example = new SadExtensions.ExamplePanel(84, 20, new Point(3, 4));
                    _example.AttachTo(_chrome);
                    _example.ShowWorking();

                    _taskCts?.Cancel();
                    _taskCts = new CancellationTokenSource();
                    _ = _example.RunAsync(TimeSpan.FromSeconds(2), _taskCts.Token);
                    break;
            }
        }

        private void RemoveBranchPanels()
        {
            if (_examplePanel != null)
            {
                _chrome.Children.Remove(_examplePanel);
                _examplePanel.Dispose();
                _examplePanel = null;
            }
        }

        private async Task RunExampleAsync(CancellationToken ct)
        {
            try
            {
                var start = DateTime.UtcNow;
                while ((DateTime.UtcNow - start).TotalSeconds < 2)
                {
                    ct.ThrowIfCancellationRequested();
                    await Task.Delay(50, ct);
                }

                _examplePanel?.Surface.Print(2, 4, "Done! Press ESC to return.", Color.Green);
            }
            catch (OperationCanceledException)
            {
                /* ignore */
            }
        }
    }
}
