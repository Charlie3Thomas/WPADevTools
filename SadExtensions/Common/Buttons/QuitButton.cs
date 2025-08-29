using SadConsole.UI.Controls;
using SadRogue.Primitives;
using WPADevTools.Messaging;

namespace WPADevTools.SadExtensions.Common.Buttons
{
    public class QuitButton
        : Button
    {
        public QuitButton()
            : base(38, 1)
        {
            Text = "Quit";
            Position = new Point(1, 5);
            Click += OnClick;
        }

        private void OnClick(object? s, EventArgs e)
            => EventHub.Publish(AppMessage.Quit());
    }
}
