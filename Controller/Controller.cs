using SadConsole;
using WPADevTools.Controller.State;
using WPADevTools.Controller.State.MainMenu;
using WPADevTools.Messaging;

namespace WPADevTools.Controller
{
    /// <summary>
    /// Singleton state machine that owns the SadConsole screen graph for your app.
    /// </summary>
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

        /// <summary>
        /// The SadConsole host (set during Initialize).
        /// </summary>
        public GameHost? Host { get; private set; }

        /// <summary>
        /// A root container where the active state's <see cref="IAppState.Root"/> is mounted.
        /// </summary>
        public ScreenObject Root { get; }

        /// <summary>
        /// The currently active state (top of the stack) or null if none.
        /// </summary>
        public IAppState? Current => _stack.Count > 0 ? _stack.Peek() : null;

        private Controller()
        {
            _states = [];
            _stack = new Stack<IAppState>();
            _sync = new object();
            Root = new ScreenObject();
            _lastUpdateTs = DateTime.UtcNow;
        }

        /// <summary>
        /// Initialize the controller with the SadConsole <see cref="GameHost"/>. Call this once at startup.
        /// </summary>
        public void Initialize(GameHost host)
        {
            if (Host != null) return;

            Host = host;
            host.FrameUpdate += OnHostUpdate;

            // Message bus wiring (Controller listens)
            EventHub.Message += OnAppMessage;

            if (host.Screen is { } screen && !ReferenceEquals(screen, Root))
                screen.Children.Add(Root);
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
            }
        }


        /// <summary>
        /// Register a state instance for later activation.
        /// </summary>
        public Controller Register<TState>(TState state) where TState : class, IAppState
        {
            lock (_sync)
            {
                _states[typeof(TState)] = state;
            }

            return this;
        }

        /// <summary>
        /// Returns a registered state instance or throws if missing.
        /// </summary>
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

        /// <summary>
        /// Replace the current state with the specified one (clears the stack and enters the new state).
        /// </summary>
        public Task GoToAsync<TState>(CancellationToken ct = default) where TState : class, IAppState
        {
            var target = Get<TState>();
            return TransitionAsync(target, replaceStack: true, ct);
        }

        /// <summary>
        /// Push a new state on top of the stack, keeping the previous one underneath.
        /// </summary>
        public Task PushAsync<TState>(CancellationToken ct = default) where TState : class, IAppState
        {
            var target = Get<TState>();
            return TransitionAsync(target, replaceStack: false, ct);
        }

        /// <summary>
        /// Pop the current state, returning to the previous one.
        /// </summary>
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
                // Exit previous if we are replacing it (GoTo) or overlaying (Push keeps previous active but hidden by default).
                if (replaceStack && previous != null)
                {
                    await ExitAsync(previous, localCt).ConfigureAwait(false);
                }

                // Mount the target visuals.
                lock (_sync)
                {
                    Root.Children.Clear();
                    Root.Children.Add(target.Root);
                }

                await target.OnEnterAsync(this, localCt).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Swallow if cancelled due to a newer transition.
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

        /// <summary>
        /// Convenience: quickly create and register a state instance using a factory.
        /// </summary>
        public TState RegisterFactory<TState>(Func<TState> factory)
            where TState : class, IAppState
        {
            var instance = factory();
            Register(instance);
            return instance;
        }
    }
}
