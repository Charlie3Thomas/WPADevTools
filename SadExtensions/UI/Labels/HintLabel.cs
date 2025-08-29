using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Labels
{
    public class HintLabel
        : Label
    {
        public HintLabel(string text)
            : base(text)
        {
            Position = new Point(0, 8);
            TextColor = Color.Gray;
        }
    }
}
