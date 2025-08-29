using WPADevTools.SadExtensions.Common.Components;

namespace WPADevTools.Controller.State.Config
{
    public readonly record struct ComposedStateConfig(
        ComposedStateSpec Spec,
        Dimensions ChromeSize
    );
}
