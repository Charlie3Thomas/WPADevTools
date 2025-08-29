using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions
{
    public class TitleLabel : Label
    {
        public TitleLabel(string text) : base(text)
        {
            Position = new Point(0, 0);
            TextColor = Color.Yellow;
        }
    }
}
