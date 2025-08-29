using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions
{
    public struct ChromeFooterBox
    {
        public void Draw(ScreenSurface surface)
        {
            surface.DrawBox(
                new Rectangle(0, surface.Height - 3, surface.Width, 3),
                ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black))
            );
            surface.Print(2, surface.Height - 2, "ESC: Back • Mouse/Keys enabled", Color.Gray);
        }
    }
}
