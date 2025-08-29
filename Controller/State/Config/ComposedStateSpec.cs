using SadConsole.UI.Controls;
using WPADevTools.SadExtensions.UI.Buttons;
using WPADevTools.SadExtensions.UI.Panels;
using WPADevTools.Messaging;

namespace WPADevTools.Controller.State.Config
{
    public readonly record struct ComposedStateSpec(
        string Title,
        string Footer,
        Func<ButtonStack, IEnumerable<Button>> ButtonsFactory,
        Func<PanelHost, IEnumerable<PanelViewBase>>? PanelsFactory = null,
        Func<bool>? BackEnabled = null,
        Func<AppMessage>? BackIntent = null
    );
}
