// Controller/State/Implementations/Azure/AzurePostDeployState.cs
using SadRogue.Primitives;
using WPADevTools.Controller.State.Config;
using WPADevTools.Messaging;
using WPADevTools.SadExtensions.UI.Panels;
using WPADevTools.Controller.State.AzureDevOps;

namespace WPADevTools.Controller.State.Implementations.Azure
{
    public sealed class AzurePostDeployState
        : ComposedStateBase
    {
        private InfoPanel? _info;

        public AzurePostDeployState()
            : base(
                name: "AzurePostDeploy",
                spec: StateConfigs.AzurePostDeploy.Spec,
                chromeSize: StateConfigs.AzurePostDeploy.ChromeSize)
        {
        }

        public override Task OnEnterAsync(Controller controller, CancellationToken ct)
        {
            var t = base.OnEnterAsync(controller, ct);

            // Start with no env selected: show header, focus first env button
            ContentHost.Clear();
            SetHeader(StateConfigs.AzurePostDeploy.Spec.Title);
            ResetMenuScrollTop();
            FocusFirstButton();

            return t;
        }

        protected override void HandleAppMessage(AppMessage msg)
        {
            if (msg.Type == AppMessageType.AdoEnvChange &&
                msg.TryGetPayload<AdoEnv>(out var env))
            {
                ShowEnv(env);
            }
        }

        private void ShowEnv(AdoEnv env)
        {
            ContentHost.Clear();
            SetHeader($"Post-Deployment • {env}");

            _info = ContentHost.Add(new InfoPanel(84, 20, new Point(3, 4),
                $"Selected environment: {env}\n\n" +
                "Here you can add post-deployment actions:\n" +
                "• Warm up services\n" +
                "• Clear caches\n" +
                "• Toggle feature flags\n" +
                "• Run smoke checks"));
        }
    }
}
