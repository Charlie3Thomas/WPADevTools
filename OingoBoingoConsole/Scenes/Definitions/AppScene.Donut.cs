using OingoBoingoConsole.ScreenExtensions;
using SadRogue.Primitives;
using SadConsole;

namespace OingoBoingoConsole.Scenes.Definitions
{
    public static partial class AppScenes
    {
        public static SceneDefinition Donut => new()
        {
            Key = SceneKey.Donut,
            Title = "ASCII Donut",
            ParentKey = SceneKey.Home,
            Factory = () =>
            {
                // Build the screen (no sliders yet)
                var screen = new CustomScreenBuilder()
                    .WithLayout(SplitLayout.None)
                    .WithSize(AppSettings.CONSOLE_WIDTH, AppSettings.CONSOLE_HEIGHT)
                    .WithHeaderFooterHeights(headerHeight: 3, footerHeight: 2)
                    .WithHeaderColor(new Color(20, 20, 60))
                    .WithFooterColor(new Color(20, 20, 60))
                    .WithPanelColors(new Color(5, 5, 5), new Color(5, 5, 5))
                    .WithBorders(true)
                    .ToggleWithF6(false)
                    .AddLabel(PanelTarget.Header, "ASCII Donut — F1: Scenes", Anchor.TopMiddle, (0, 1))
                    .Build();

                // Add the donut (auto-sizes to the center panel)
                var donut = new DonutPanel(
                    Math.Max(8, screen.ContentInnerRect.Width),
                    Math.Max(4, screen.ContentInnerRect.Height))
                {
                };

                screen.AddSurfaceObject(
                    PanelTarget.A, donut,
                    Anchor.Middle, (0, 0),
                    clampInside: true, autoSizeToPanel: true);

                return screen;
            }
        };
    }
}
