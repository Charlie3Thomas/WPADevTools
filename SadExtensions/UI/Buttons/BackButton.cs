using WPADevTools.Messaging;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class BackButton
        : StackButton
    {
        public BackButton(ButtonStack stack, string text = "Back")
            : base(stack, 38, text)
        {
            Click += (_, __) =>
                EventHub.Publish(AppMessage.BackRequested());
        }
    }
}