namespace WPADevTools.Messaging
{
    public static class EventHub
    {
        public static event Action<AppMessage>? Message;

        public static void Publish(AppMessage message) => 
            Message?.Invoke(message);
    }
}
