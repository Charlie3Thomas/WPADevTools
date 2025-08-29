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

        public static AppMessage BranchChange(object branch) => new(AppMessageType.BranchChange, branch);
        public static AppMessage QuitRequested() => new(AppMessageType.QuitRequested, null);
        public static AppMessage Quit() => new(AppMessageType.Quit, null);
        public static AppMessage ToastDismiss() => new(AppMessageType.ToastDismiss, null);

        public bool TryGetPayload<T>(out T value)
        {
            if (Payload is T t) { value = t; return true; }
            value = default!;
            return false;
        }
    }
}
