// Controller/State/Implementations/Azure/AzureDevOpsToolsState.cs
using WPADevTools.Controller.State.Config;

namespace WPADevTools.Controller.State.Implementations.Azure
{
    public sealed class AzureDevOpsToolsState : ComposedStateBase
    {
        public AzureDevOpsToolsState()
            : base(
                name: "AzureDevOpsTools",
                spec: StateConfigs.AzureTools.Spec,
                chromeSize: StateConfigs.AzureTools.ChromeSize)
        {
        }
    }
}
