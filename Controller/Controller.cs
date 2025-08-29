using SadConsole;
using WPADevTools.Controller.State;
using WPADevTools.Controller.State.MainMenu;
using WPADevTools.Messaging;

namespace WPADevTools.Controller
{
    public sealed class Controller
    {
        private static readonly Lazy<Controller> _lazy = new(() => new Controller());
        public static Controller Instance => _lazy.Value;

        private readonly Dictionary<Type, IAppState> _states;
        private readonly Stack<IAppState> _stack;
        private readonly object _sync;

        private CancellationTokenSource? _transitionCts;
        private DateTime _lastUpdateTs;


        private Branch _branch = Branch.Menu;

        public event Action<Branch>? BranchChanged;

        public Branch Branch
        {
            get => _branch;
            set
            {
                if (_branch == value) return;
                _branch = value;
                BranchChanged?.Invoke(value);
            }
        }

        public GameHost? Host { get; private set; }

        public ScreenObject Root { get; }

        public IAppState? Current => _stack.Count > 0 ? _stack.Peek() : null;

        private Controller()
        {
            _states = [];
            _stack = new Stack<IAppState>();
            _sync = new object();
            Root = new ScreenObject();
            _lastUpdateTs = DateTime.UtcNow;
        }

        public void Initialize(GameHost host)
        {
            if (Host != null) return;

            Host = host;
            host.FrameUpdate += OnHostUpdate;

            EventHub.Message += OnAppMessage;

            if (host.Screen is { } screen && !ReferenceEquals(screen, Root))
                screen.Children.Add(Root);
        }

        public void Shutdown()
        {
            if (Host != null)
            {
                Host.FrameUpdate -= OnHostUpdate;
                Host = null;
            }
            EventHub.Message -= OnAppMessage;
        }

        private void OnAppMessage(AppMessage msg)
        {
            switch (msg.Type)
            {
                case AppMessageType.Quit:
                    SadConsole.Game.Instance?.MonoGameInstance.Exit();
                    break;

                case AppMessageType.BranchChange:
                    if (msg.TryGetPayload<Branch>(out var branch))
                        Branch = branch;
                    break;

                case AppMessageType.GoToState:
                    if (msg.TryGetPayload<Type>(out var stateType))
                        _ = GoToTypeAsync(stateType);
                    break;
            }
        }

        private Task GoToTypeAsync(Type stateType, CancellationToken ct = default)
        {
            IAppState target;
            lock (_sync)
            {
                if (!_states.TryGetValue(stateType, out var state))
                    throw new KeyNotFoundException($"State not registered: {stateType.Name}");
                target = state;
            }
            return TransitionAsync(target, replaceStack: true, ct);
        }

        public Controller Register<TState>(TState state) where TState : class, IAppState
        {
            lock (_sync)
            {
                _states[typeof(TState)] = state;
            }

            return this;
        }

        public TState Get<TState>() where TState : class, IAppState
        {
            lock (_sync)
            {
                if (_states.TryGetValue(typeof(TState), out var state))
                {
                    return (TState)state;
                }
            }

            throw new KeyNotFoundException($"State not registered: {typeof(TState).Name}");
        }

        public Task GoToAsync<TState>(CancellationToken ct = default) where TState : class, IAppState
        {
            var target = Get<TState>();
            return TransitionAsync(target, replaceStack: true, ct);
        }

        public Task PushAsync<TState>(CancellationToken ct = default) where TState : class, IAppState
        {
            var target = Get<TState>();
            return TransitionAsync(target, replaceStack: false, ct);
        }

        public async Task PopAsync(CancellationToken ct = default)
        {
            IAppState? leaving;

            lock (_sync)
            {
                if (_stack.Count == 0)
                {
                    return;
                }

                leaving = _stack.Pop();
            }

            await ExitAsync(leaving!, ct).ConfigureAwait(false);

            lock (_sync)
            {
                Root.Children.Clear();

                if (_stack.Count > 0)
                {
                    var next = _stack.Peek();
                    Root.Children.Add(next.Root);
                }
            }
        }

        private async Task TransitionAsync(IAppState target, bool replaceStack, CancellationToken ct)
        {
            if (Host == null)
            {
                throw new InvalidOperationException("Controller.Initialize(host) must be called before transitions.");
            }

            CancelInFlightTransition();
            _transitionCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var localCt = _transitionCts.Token;

            IAppState? previous = null;

            lock (_sync)
            {
                if (replaceStack)
                {
                    _stack.Clear();
                }
                else
                {
                    previous = _stack.Count > 0 ? _stack.Peek() : null;
                }

                _stack.Push(target);
            }

            try
            {
                if (replaceStack && previous != null)
                {
                    await ExitAsync(previous, localCt).ConfigureAwait(false);
                }

                lock (_sync)
                {
                    Root.Children.Clear();
                    Root.Children.Add(target.Root);
                }

                await target.OnEnterAsync(this, localCt).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Swallow
            }
        }

        private async Task ExitAsync(IAppState state, CancellationToken ct)
        {
            try
            {
                await state.OnExitAsync(this, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void CancelInFlightTransition()
        {
            try
            {
                _transitionCts?.Cancel();
            }
            finally
            {
                _transitionCts?.Dispose();
                _transitionCts = null;
            }
        }

        private void OnHostUpdate(object? sender, SadConsole.GameHost e)
        {
            var now = DateTime.UtcNow;
            var delta = now - _lastUpdateTs;
            _lastUpdateTs = now;

            var current = Current;
            current?.Update(delta);
        }

        public TState RegisterFactory<TState>(Func<TState> factory)
            where TState : class, IAppState
        {
            var instance = factory();
            Register(instance);
            return instance;
        }
    }
}
