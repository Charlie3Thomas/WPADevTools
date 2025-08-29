using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Layout
{
    public static class Anchors
    {
        public static Point Center(ScreenSurface parent, int childWidth, int childHeight, int offsetX = 0, int offsetY = 0)
        {
            var x = (parent.Width - childWidth) / 2 + offsetX;
            var y = (parent.Height - childHeight) / 2 + offsetY;
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            return new Point(x, y);
        }
    }
}
