using WPADevTools.Messaging;
using WPADevTools.Controller.State.MainMenu;
using WPADevTools.SadExtensions.UI.Buttons;
using WPADevTools.SadExtensions.Common.Components;

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
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new ExampleButton(stack),
                    new QuitButton(stack)
                ],
                PanelsFactory: null,
                BackEnabled: () => Controller.Instance.Branch != Branch.Menu,
                BackIntent: () => AppMessage.BranchChange(Branch.Menu)
            ),
            ChromeSize: DefaultChrome
        );
    }
}
