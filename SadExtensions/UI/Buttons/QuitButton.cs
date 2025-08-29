using System;
using WPADevTools.Messaging;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class QuitButton : StackButton
    {
        public QuitButton(ButtonStack stack) : base(stack, 38, "Quit")
        {
            Click += (_, __) => EventHub.Publish(AppMessage.QuitRequested());
        }
    }
}
