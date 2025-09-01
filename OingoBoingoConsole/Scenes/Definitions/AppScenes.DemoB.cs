using OingoBoingoConsole.ScreenExtensions;
using SadRogue.Primitives;

namespace OingoBoingoConsole.Scenes.Definitions
{
    // AppScenes::DemoB
    public static partial class AppScenes
    {
        public static SceneDefinition DemoB = new()
        {
            Key = SceneKey.DemoB,
            Title = "Second Scene (Horizontal)",
            ParentKey = SceneKey.DemoA, // auto “< Back” button will appear
            Factory = () => new CustomScreenBuilder()
                .WithLayout(SplitLayout.Horizontal)
                .WithSize(AppSettings.CONSOLE_WIDTH, AppSettings.CONSOLE_HEIGHT)
                .WithPanelColors(new Color(15, 35, 65), new Color(65, 20, 20))
                .WithBorders(true)
                .ToggleWithF6(true)
                .AddLabel(PanelTarget.A, "Scene B — Top", Anchor.TopLeft, (1, 1))
                .AddLabel(PanelTarget.B, "Scene B — Bottom", Anchor.TopLeft, (1, 1))
                .AddButton(PanelTarget.B, "Go back?", Anchor.BottomRight, (-12, -2), 12)
                .Build()
        };
    }
}
