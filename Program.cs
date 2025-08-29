using SadConsole;
using SadConsole.Configuration;
using WPADevTools.Controller.State.Implementations.MainMenu;

namespace WPADevTools
{
    internal class Program
    {
        static void Main()
        {
            Settings.WindowTitle = "WPA Dev Tools";

            var startup = new Builder()
                .SetWindowSizeInCells(90, 30)
                .SetStartingScreen(CreateRoot)
                .OnStart((sender, host) =>
                {
                    Controller.Controller.Instance.RegisterFactory(() => new MainMenuState());
                    _ = Controller.Controller.Instance.GoToAsync<MainMenuState>();
                })
                .ConfigureFonts(true);

            Game.Create(startup);
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        private static IScreenObject CreateRoot(GameHost host)
        {
            Controller.Controller.Instance.Initialize(host);
            return Controller.Controller.Instance.Root;
        }
    }
}
