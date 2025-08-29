using System;
using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Adornments
{
    public readonly struct ChromeHeaderBox
    {
        private readonly string _message;
        public ChromeHeaderBox(string message) => _message = message ?? string.Empty;

        public void Draw(ScreenSurface surface)
        {
            surface.DrawBox(
                new Rectangle(0, 0, surface.Width, 3),
                ShapeParameters.CreateStyledBox(
                    ICellSurface.ConnectedLineThin,
                    new ColoredGlyph(Color.White, Color.Black)));

            WriteText(surface, _message);
        }

        public static void WriteText(ScreenSurface surface, string text)
        {
            const int left = 2, right = 2, y = 1;
            var width = Math.Max(0, surface.Width - (left + right));
            if (text.Length > width) text = text[..width];
            surface.Surface.Print(left, y, text.PadRight(width), Color.Yellow);
        }
    }
}
