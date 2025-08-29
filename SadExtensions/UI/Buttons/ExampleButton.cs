using WPADevTools.Controller.State.MainMenu;
using WPADevTools.Messaging;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class ExampleButton : StackButton
    {
        public ExampleButton(ButtonStack stack) : base(stack, 38, "Run Example Task")
        {
            Click += OnClick;
        }

        private void OnClick(object? s, EventArgs e)
            => EventHub.Publish(AppMessage.BranchChange(Branch.ExampleTask));
    }
}
