using WPADevTools.Messaging;
using WPADevTools.Controller.State.AzureDevOps;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public sealed class AdoEnvButton
        : StackButton
    {
        public AdoEnv Env { get; }
        public AdoEnvButton(ButtonStack stack, AdoEnv env)
            : base(stack, 38, env.ToString())
        {
            Env = env;

            Click += (_, __) =>
                EventHub.Publish(AppMessage.AdoEnvChange(env));
        }
    }
}