using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Panels
{
    public class FramedPanel
        : PanelViewBase
    {
        public FramedPanel(int width, int height, Point position)
            : base(width, height, position)
        {
            Surface.Surface.Fill(Color.Black, Color.Black, 0);
            Surface.DrawBox(
                new Rectangle(0, 0, width, height),
                ShapeParameters.CreateStyledBox(
                    ICellSurface.ConnectedLineThin,
                    new ColoredGlyph(Color.White, Color.Black)));
        }

        public void Print(int x, int y, string text, Color color)
            => Surface.Surface.Print(x, y, text, color);
    }
}
