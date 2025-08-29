using WPADevTools.Messaging;
using WPADevTools.Controller.State;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class NavigateToStateButton<TState>
        : StackButton
        where TState : class, IAppState
    {
        public NavigateToStateButton(ButtonStack stack, string text, int width = 38)
            : base(stack, width, text)
        {
            Click += (_, __) =>
                EventHub.Publish(AppMessage.GoToState<TState>());
        }
    }
}