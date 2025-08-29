using WPADevTools.Controller.State;

namespace WPADevTools.Messaging
{
    public readonly struct AppMessage
    {
        public AppMessageType Type { get; }
        public object? Payload { get; }

        private AppMessage(AppMessageType type, object? payload)
        {
            Type = type;
            Payload = payload;
        }

        public static AppMessage BranchChange(object branch) =>
            new(AppMessageType.BranchChange, branch);

        public static AppMessage BackRequested() =>
            new(AppMessageType.BackRequested, null);

        public static AppMessage QuitRequested() =>
            new(AppMessageType.QuitRequested, null);

        public static AppMessage Quit() =>
            new(AppMessageType.Quit, null);

        public static AppMessage ToastDismiss() =>
            new(AppMessageType.ToastDismiss, null);

        public static AppMessage GoToState<TState>()
            where TState : class, IAppState =>
            new(AppMessageType.GoToState, typeof(TState));

        public static AppMessage AdoEnvChange(object env) =>
            new(AppMessageType.AdoEnvChange, env);

        public bool TryGetPayload<T>(out T value)
        {
            if (Payload is T t) { value = t; return true; }
            value = default!;
            return false;
        }
    }
}
