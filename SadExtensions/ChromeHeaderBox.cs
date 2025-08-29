using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions
{
    public struct ChromeHeaderBox
    {
        public void Draw(ScreenSurface surface)
        {
            surface.DrawBox(
                new Rectangle(0, 0, surface.Width, 3),
                ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black))
            );
            surface.Print(2, 1, "Main Menu", Color.Yellow);
        }
    }
}
