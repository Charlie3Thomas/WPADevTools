using SadConsole.UI.Controls;
using SadRogue.Primitives;
using WPADevTools.SadExtensions;
using WPADevTools.SadExtensions.MainMenu;

namespace WPADevTools.Controller.State.Implementations.MainMenu
{
    public sealed partial class MainMenuState
        : AppStateBase
    {
        private readonly ChromeSurface _chrome;
        private readonly MainMenuConsole _menuUi;

        private Branch _active;
        private Button? _btnExample;
        private Button? _btnQuit;

        public MainMenuState()
            : base(Branch.Menu.ToString())
        {
            _chrome = new ChromeSurface(90, 30);
            Root.Children.Add(_chrome);

            _menuUi = new MainMenuConsole();
            _chrome.Children.Add(_menuUi);

            BuildMenu();
            new ChromeHeaderBox().Draw(_chrome);
            new ChromeFooterBox().Draw(_chrome);
        }

        public override Task OnEnterAsync(Controller controller, CancellationToken ct)
        {
            _active = Branch.Menu;
            _menuUi.IsVisible = true;
            _menuUi.IsEnabled = true;
            _menuUi.IsFocused = true;
            return Task.CompletedTask;
        }

        private void BuildMenu()
        {
            _menuUi.Controls.Clear();

            _menuUi.FocusOnMouseClick = true;

            var title = new Label("WPA Dev Tools")
            {
                Position = new Point(0, 0),
                TextColor = Color.Yellow
            };
            _menuUi.Controls.Add(title);

            _btnExample = new Button(38, 1) { Text = "Run Example Task", Position = new Point(1, 3) };
            _btnExample.Click += (_, __) => SwitchBranch(Branch.ExampleTask);
            _menuUi.Controls.Add(_btnExample);

            _btnQuit = new Button(38, 1) { Text = "Quit", Position = new Point(1, 5) };
            _btnQuit.Click += (_, __) => SadConsole.Game.Instance?.MonoGameInstance.Exit();
            _menuUi.Controls.Add(_btnQuit);

            var hint = new Label("Enter/Click to select • ESC: Back")
            {
                Position = new Point(0, 8),
                TextColor = Color.Gray
            };

            _menuUi.Controls.Add(hint);

            _menuUi.IsFocused = true;
            _btnExample.IsFocused = true;
        }
    }
}
