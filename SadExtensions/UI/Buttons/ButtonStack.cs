using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    /// <summary>
    /// Lays out buttons vertically inside a ControlsConsole.
    /// </summary>
    public sealed class ButtonStack
    {
        private readonly ControlsConsole _console;
        private readonly int _x;
        private readonly int _startY;
        private readonly int _gapY;
        private int _count;

        public ButtonStack(ControlsConsole console, int x = 1, int startY = 3, int gapY = 2)
        {
            _console = console;
            _x = x;
            _startY = startY;
            _gapY = gapY;
        }

        /// <summary>Places the button at the next slot and adds to the console.</summary>
        public T Add<T>(T button) where T : Button
        {
            var y = _startY + _count * _gapY;
            button.Position = new Point(_x, y);
            _console.Controls.Add(button);
            _count++;
            return button;
        }

        /// <summary>The Y for the next item (useful for placing hints below buttons).</summary>
        public int NextY => _startY + _count * _gapY;

        public void Reset() => _count = 0;
    }
}
