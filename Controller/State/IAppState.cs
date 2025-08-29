using SadConsole;

namespace WPADevTools.Controller.State
{
    public interface IAppState
    {
        string Name { get; }

        ScreenObject Root { get; }

        Task OnEnterAsync(Controller controller, CancellationToken ct);

        Task OnExitAsync(Controller controller, CancellationToken ct);

        void Update(TimeSpan delta);
    }
}
