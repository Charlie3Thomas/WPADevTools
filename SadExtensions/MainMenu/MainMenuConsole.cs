using SadConsole.UI;
using SadRogue.Primitives;
using WPADevTools.SadExtensions.Common.Components;

namespace WPADevTools.SadExtensions.MainMenu
{
    public class MainMenuConsole
        : ControlsConsole
    {
        private readonly static Dimensions ConsoleDimensions = new(40, 12);
        private readonly static Point ConsolePoint = new(3, 4);

        public MainMenuConsole()
            : base(ConsoleDimensions.Width, ConsoleDimensions.Height)
        {
            Position = ConsolePoint;
            UseKeyboard = true;
            UseMouse = true;
        }
    }
}
