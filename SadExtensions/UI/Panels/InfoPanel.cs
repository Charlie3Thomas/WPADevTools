// SadExtensions/UI/Panels/InfoPanel.cs
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Panels
{
    public sealed class InfoPanel
        : FramedPanel
    {
        public InfoPanel(int width, int height, Point position, string text)
            : base(width, height, position)
        {
            var lines = (text ?? string.Empty).Split('\n');
            int y = 2;

            foreach (var line in lines)
            {
                Print(2, y++, line, Color.White);
            }
        }
    }
}
