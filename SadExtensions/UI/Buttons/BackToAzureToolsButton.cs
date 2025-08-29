using WPADevTools.Messaging;
using WPADevTools.Controller.State.Implementations.Azure;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class BackToAzureToolsButton
        : StackButton
    {
        public BackToAzureToolsButton(ButtonStack stack)
            : base(stack, 38, "Back")
        {
            Click += (_, __) => EventHub.Publish(AppMessage.GoToState<AzureDevOpsToolsState>());
        }
    }
}