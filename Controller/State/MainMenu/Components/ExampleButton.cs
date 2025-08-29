using SadConsole.UI.Controls;
using SadRogue.Primitives;
using WPADevTools.Messaging;

namespace WPADevTools.Controller.State.MainMenu.Components
{
    public class ExampleButton
        : Button
    {
        public ExampleButton()
            : base(38, 1)
        {
            Text = "Run Example Task";
            Position = new Point(1, 3);
            Click += OnClick;
        }

        private void OnClick(object? s, EventArgs e)
            => EventHub.Publish(AppMessage.BranchChange(Branch.ExampleTask));
    }
}
