using WPADevTools.Messaging;
using WPADevTools.Controller.State.Implementations.MainMenu;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class BackToMainMenuButton
        : StackButton
    {
        public BackToMainMenuButton(ButtonStack stack)
            : base(stack, 38, "Back")
        {
            Click += (_, __) => EventHub.Publish(AppMessage.GoToState<MainMenuState>());
        }
    }
}