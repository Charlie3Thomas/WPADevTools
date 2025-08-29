using WPADevTools.Messaging;
using WPADevTools.Controller.State.MainMenu;
using WPADevTools.SadExtensions.UI.Buttons;
using WPADevTools.SadExtensions.Common.Components;
using WPADevTools.Controller.State.AzureDevOps;
using WPADevTools.Controller.State.Implementations.Azure;

namespace WPADevTools.Controller.State.Config
{
    public static class StateConfigs
    {
        public static readonly Dimensions DefaultChrome = new(90, 30);

        public static readonly ComposedStateConfig MainMenu = new(
            new ComposedStateSpec(
                Title: "Developer Tools:",
                Footer: "ESC: Back  •  Mouse/Keys enabled",
                ButtonsFactory: stack =>
                [
                    new NavigateToStateButton<AzureDevOpsToolsState>(stack, "Azure DevOps Tools"),
                    new QuitButton(stack)
                ],
                PanelsFactory: null,
                BackEnabled: () => Controller.Instance.Branch != Branch.Menu,
                BackIntent: () => AppMessage.BranchChange(Branch.Menu)
            ),
            ChromeSize: DefaultChrome
        );

        public static readonly ComposedStateConfig AzureTools = new(
            new ComposedStateSpec(
                Title: "Azure DevOps Tools",
                Footer: "ESC: Back  •  Mouse/Keys enabled",
                ButtonsFactory: stack =>
                [
                    new NavigateToStateButton<AzurePostDeployState>(stack, "Post-Deployment Scripting"),
                    new BackButton(stack),
                    new QuitButton(stack),
                ],
                PanelsFactory: null,
                BackEnabled: () => true,
                BackIntent: () => AppMessage.GoToState<Implementations.MainMenu.MainMenuState>()
            ),
            ChromeSize: DefaultChrome
        );

        public static readonly ComposedStateConfig AzurePostDeploy = new(
            new ComposedStateSpec(
                Title: "Azure DevOps: Post-Deployment",
                Footer: "ESC: Back  •  Mouse/Keys enabled",
                ButtonsFactory: stack =>
                [
                    new AdoEnvButton(stack, AdoEnv.Debug),
                    new AdoEnvButton(stack, AdoEnv.Develop),
                    new AdoEnvButton(stack, AdoEnv.Staging),
                    new AdoEnvButton(stack, AdoEnv.Production),
                    new BackButton(stack),
                    new QuitButton(stack),
                ],
                PanelsFactory: null,
                BackEnabled: () => true,
                BackIntent: () => AppMessage.GoToState<AzureDevOpsToolsState>()
            ),
            ChromeSize: DefaultChrome
        );
    }
}
