using OingoBoingoConsole.ScreenExtensions;
using SadConsole.UI.Controls;
using SadConsole;
using SadRogue.Primitives;

namespace OingoBoingoConsole.Scenes.Definitions
{
    // AppScenes::DemoA
    public static partial class AppScenes
    {
        public static SceneDefinition DemoA = new()
        {
            Key = SceneKey.DemoA,
            Title = "Common Controls",
            Factory = () => new CustomScreenBuilder()
                .WithLayout(SplitLayout.Vertical)
                .WithSize(AppSettings.CONSOLE_WIDTH, AppSettings.CONSOLE_HEIGHT)
                .WithPanelColors(new Color(30, 70, 120), new Color(120, 40, 40))
                .WithBorders(true)
                .ToggleWithF6(true)

                // Panel A
                .AddLabel(PanelTarget.A, "Common Controls", Anchor.TopLeft, (1, 1))
                .AddTextBox(PanelTarget.A, 18, "Edit me", Anchor.TopMiddle, (0, 1))
                .AddComboBox(PanelTarget.A, 16, 20, 6, ["Alpha", "Beta", "Gamma"], 0, Anchor.TopRight, (-1, 1))
                .AddListBox(PanelTarget.A, 20, 7, ["One", "Two", "Three"], Anchor.MiddleLeft, (1, 0))
                .AddDrawingArea(PanelTarget.A, 20, 6, Anchor.MiddleRight, (-21, -3))
                .AddButton(PanelTarget.A, "OK", Anchor.BottomLeft, (-1, -1), 10)
                .AddCheckBox(PanelTarget.A, "Check me", true, Anchor.BottomLeft, (12, -1))
                .AddRadioButton(PanelTarget.A, "Pick me", true, Anchor.BottomLeft, (29, -1))
                .AddToggleSwitch(PanelTarget.A, "Toggle", false, Anchor.BottomLeft, (47, -1))
                .AddNumberBox(PanelTarget.A, 8, 42, 0, 100, false, true, Anchor.BottomMiddle, (-20, -1))
                .AddProgressBar(PanelTarget.A, 24, 0.35f, HorizontalAlignment.Left, Anchor.BottomMiddle, (6, -1))
                .AddSelectionButton(PanelTarget.A, "Select…", Anchor.BottomMiddle, (-7, -3))

                // Panel B
                .AddButton3d(PanelTarget.B, "3D Btn", 12, 3, Anchor.TopLeft, (1, 1))
                .AddButtonBox(PanelTarget.B, "Box Btn", 12, 3, Anchor.TopRight, (-13, 1))
                .AddPanel(PanelTarget.B, 26, 8, Anchor.MiddleLeft, (1, -4))
                .AddSurfaceViewer(PanelTarget.B, 24, 6, new CellSurface(24, 6), Anchor.MiddleRight, (-26, -3))
                .AddScrollBar(PanelTarget.B, Orientation.Vertical, 8, Anchor.MiddleRight, (-1, -4))
                .AddTabControl(
                    PanelTarget.B, 20, 6,
                    [new TabItem("One", new Panel(18, 5)), new TabItem("Two", new Panel(18, 5))],
                    Anchor.BottomMiddle, (-10, -6))
                .Build()
        };
    }
}
