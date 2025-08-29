namespace WPADevTools.Messaging
{
    public enum AppMessageType
    {
        BranchChange,
        QuitRequested,   // user clicked Quit
        Quit,            // confirmed quit
        ToastDismiss     // dismiss an open toast
    }
}
