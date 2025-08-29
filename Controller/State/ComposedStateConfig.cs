using WPADevTools.SadExtensions.Common.Components;

namespace WPADevTools.Controller.State
{
    /// <summary>Full configuration for a ComposedStateBase-derived state.</summary>
    public readonly record struct ComposedStateConfig(
        ComposedStateSpec Spec,
        Dimensions ChromeSize
    );
}
