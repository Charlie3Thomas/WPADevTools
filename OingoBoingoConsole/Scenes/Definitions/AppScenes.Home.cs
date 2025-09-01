using OingoBoingoConsole.ScreenExtensions;
using SadConsole;
using SadRogue.Primitives;

namespace OingoBoingoConsole.Scenes.Definitions
{
    // AppScenes::Home
    public static partial class AppScenes
    {
        public static SceneDefinition Home => new()
        {
            Key = SceneKey.Home,
            Title = "Main Menu",
            ParentKey = null,
            Factory = () =>
            {
                // convert pixels to cells using the current font
                var glyphH = GameHost.Instance?.DefaultFont?.GlyphHeight ?? 16; // fallback
                int headerCells = Math.Max(1, (int)Math.Round(64.0 / glyphH));  // 48 px header
                int footerCells = Math.Max(1, (int)Math.Round(32.0 / glyphH));  // 24 px footer

                return new CustomScreenBuilder()
                    .WithLayout(SplitLayout.None)
                    .WithSize(AppSettings.CONSOLE_WIDTH, AppSettings.CONSOLE_HEIGHT)
                    .WithHeaderFooterHeights(headerCells, footerCells)   // <— here
                    .WithPanelColors(new Color(255, 0, 0), new Color(0, 255, 0))
                    .WithBorders(true)
                    .ToggleWithF6(false)
                    .AddLabel(PanelTarget.Header, "Oingo Boingo, Motherfucker", Anchor.TopMiddle, (0, 1))
                    .Build();
            }
        };

    }
}
