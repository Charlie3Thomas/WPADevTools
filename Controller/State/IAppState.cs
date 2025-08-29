using SadConsole;

namespace WPADevTools.Controller.State
{
    /// <summary>
    /// Minimal state interface for SadConsole states.
    /// Implementations can directly inherit from <see cref="ScreenObject"/> or wrap one.
    /// </summary>
    public interface IAppState
    {
        /// <summary>
        /// A short, unique name for the state (used for logging/lookup).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The root visual for this state. Typically a <see cref="ScreenObject"/> tree.
        /// </summary>
        ScreenObject Root { get; }

        /// <summary>
        /// Called when the state becomes active.
        /// Do heavy work asynchronously (asset loads, scene setup, etc.).
        /// </summary>
        Task OnEnterAsync(Controller controller, CancellationToken ct);

        /// <summary>
        /// Called when the state is deactivated. Dispose transient resources here.
        /// </summary>
        Task OnExitAsync(Controller controller, CancellationToken ct);

        /// <summary>
        /// Per-frame update hook (optional). Delta is the elapsed frame time.
        /// </summary>
        void Update(TimeSpan delta);
    }
}
