using WPADevTools.Messaging;
using WPADevTools.Controller.State.Implementations.Azure;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class AzureDevOpsToolsButton
        : StackButton
    {
        public AzureDevOpsToolsButton(ButtonStack stack)
            : base(stack, 38, "Azure DevOps Tools")
        {
            Click += (_, __) => EventHub.Publish(AppMessage.GoToState<AzureDevOpsToolsState>());
        }
    }
}