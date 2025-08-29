using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.Common.Buttons
{
    public class QuitButton : Button
    {
        public QuitButton()
            : base(38, 1)
        {
            Text = "Quit";
            Position = new Point(1, 5);
        }
    }
}
