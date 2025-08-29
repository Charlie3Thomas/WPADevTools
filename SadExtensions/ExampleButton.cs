using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions
{
    public class ExampleButton : Button
    {
        public ExampleButton()
            : base(38, 1)
        {
            Text = "Run Example Task";
            Position = new Point(1, 3);
        }
    }
}
