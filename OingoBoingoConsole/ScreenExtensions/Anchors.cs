// File: ScreenExtensions/Anchors.cs
using SadConsole;
using SadRogue.Primitives;

namespace OingoBoingoConsole.ScreenExtensions
{
    public enum Anchor
    {
        TopLeft,
        TopRight,
        MiddleLeft,
        MiddleRight,
        TopMiddle,
        BottomMiddle,
        BottomLeft,
        BottomRight,
        Middle
    }

    public static class AnchorLayout
    {
        /// <summary>
        /// Calculates a position inside <paramref name="area"/> for a child of size <paramref name="size"/>
        /// anchored to <paramref name="anchor"/>, with optional <paramref name="offset"/>.
        /// </summary>
        public static Point GetPosition(Rectangle area, Point size, Anchor anchor, Point? offset = null, bool clampInside = true)
        {
            int x, y;

            switch (anchor)
            {
                case Anchor.TopLeft:
                    x = area.X; y = area.Y; break;

                case Anchor.TopRight:
                    x = area.X + area.Width - size.X; y = area.Y; break;

                case Anchor.BottomLeft:
                    x = area.X; y = area.Y + area.Height - size.Y; break;

                case Anchor.BottomRight:
                    x = area.X + area.Width - size.X; y = area.Y + area.Height - size.Y; break;

                case Anchor.TopMiddle:
                    x = area.X + (area.Width - size.X) / 2; y = area.Y; break;

                case Anchor.BottomMiddle:
                    x = area.X + (area.Width - size.X) / 2; y = area.Y + area.Height - size.Y; break;

                case Anchor.MiddleLeft:
                    x = area.X; y = area.Y + (area.Height - size.Y) / 2; break;

                case Anchor.MiddleRight:
                    x = area.X + area.Width - size.X; y = area.Y + (area.Height - size.Y) / 2; break;

                case Anchor.Middle:
                default:
                    x = area.X + (area.Width - size.X) / 2;
                    y = area.Y + (area.Height - size.Y) / 2;
                    break;
            }

            if (offset.HasValue)
            {
                x += offset.Value.X;
                y += offset.Value.Y;
            }

            var pos = new Point(x, y);
            return clampInside ? ClampInside(pos, size, area) : pos;
        }

        private static Point ClampInside(Point pos, Point size, Rectangle area)
        {
            int minX = area.X;
            int minY = area.Y;
            int maxX = area.X + Math.Max(0, area.Width - size.X);
            int maxY = area.Y + Math.Max(0, area.Height - size.Y);

            int cx = pos.X < minX ? minX : (pos.X > maxX ? maxX : pos.X);
            int cy = pos.Y < minY ? minY : (pos.Y > maxY ? maxY : pos.Y);

            return new Point(cx, cy);
        }
    }

    public static class ScreenObjectAnchorExtensions
    {
        /// <summary>
        /// Positions <paramref name="child"/> inside <paramref name="hostArea"/> using <paramref name="anchor"/>.
        /// </summary>
        public static void AttachToAnchor(this IScreenObject child, Rectangle hostArea, Anchor anchor, Point? offset = null, bool clampInside = true)
        {
            var size = GetChildSize(child);
            child.Position = AnchorLayout.GetPosition(hostArea, size, anchor, offset, clampInside);
        }

        public static void AttachToAnchorWithPadding(
                this IScreenObject obj,
                Rectangle hostArea,
                Anchor anchor,
                (int x, int y)? offset = null,
                (int left, int top, int right, int bottom)? pad = null)
        {
            var p = pad ?? (0, 0, 0, 0);
            var padded = new Rectangle(
                hostArea.X + p.left,
                hostArea.Y + p.top,
                Math.Max(0, hostArea.Width - p.left - p.right),
                Math.Max(0, hostArea.Height - p.top - p.bottom));

            obj.AttachToAnchor(padded, anchor, offset);
        }

        private static Point GetChildSize(IScreenObject child) =>
            child switch
            {
                SadConsole.Console con => new Point(con.Width, con.Height),
                IScreenSurface surf => new Point(surf.Surface.ViewWidth, surf.Surface.ViewHeight),
                _ => new Point(1, 1)
            };
    }
}
