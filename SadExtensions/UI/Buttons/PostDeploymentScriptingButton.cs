using WPADevTools.Messaging;
using WPADevTools.Controller.State.Implementations.Azure;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class PostDeploymentScriptingButton
        : StackButton
    {
        public PostDeploymentScriptingButton(ButtonStack stack)
            : base(stack, 38, "Post-Deployment Scripting")
        {
            Click += (_, __) => EventHub.Publish(AppMessage.GoToState<AzurePostDeployState>());
        }
    }
}