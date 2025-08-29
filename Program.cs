using SadConsole;
using SadConsole.Configuration;
using WPADevTools.Controller.State.Implementations.MainMenu;
using WPADevTools.Controller.State.Implementations.Azure;

namespace WPADevTools
{
    internal class Program
    {
        private const string Title = "WPA Dev Tools";

        static void Main()
        {
            Settings.WindowTitle = Title;

            var startup = new Builder()
                .SetWindowSizeInCells(90, 30)
                .SetStartingScreen(CreateRoot)
                .OnStart((sender, host) =>
                {
                    Controller.Controller.Instance.RegisterFactory(() => new MainMenuState());
                    Controller.Controller.Instance.RegisterFactory(() => new AzureDevOpsToolsState());
                    Controller.Controller.Instance.RegisterFactory(() => new AzurePostDeployState());

                    _ = Controller.Controller.Instance.GoToAsync<MainMenuState>();
                }).ConfigureFonts(true);

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
