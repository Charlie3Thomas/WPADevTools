using SadConsole;

namespace WPADevTools.Controller.State
{
    public abstract class AppStateBase
        : IAppState
    {
        public string Name { get; }
        public ScreenObject Root { get; }

        protected AppStateBase(string name)
        {
            Name = name;
            Root = new ScreenObject();
        }

        public virtual Task OnEnterAsync(Controller controller, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnExitAsync(Controller controller, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public virtual void Update(TimeSpan delta)
        {
        }
    }
}
