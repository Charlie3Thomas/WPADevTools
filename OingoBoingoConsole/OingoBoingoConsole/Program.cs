using OingoBoingoConsole.Scenes;
using OingoBoingoConsole.Scenes.Definitions;
using SadConsole;
using SadConsole.Configuration;

namespace OingoBoingoConsole
{
    internal class Program
    {
        private const string Title = "WPA Dev Tools";

        static void Main()
        {
            Settings.WindowTitle = Title;

            var defs = AppScenes.Build();

            var startup = new Builder()
                .SetWindowSizeInCells(AppSettings.CONSOLE_WIDTH, AppSettings.CONSOLE_HEIGHT)
                .SetStartingScreen(_ => new SceneHost(defs, initialKey: AppScenes.Initial))
                .IsStartingScreenFocused(true)
                .ConfigureFonts(true);

            Game.Create(startup);
            Game.Instance.Run();
            Game.Instance.Dispose();
        }
    }
}
