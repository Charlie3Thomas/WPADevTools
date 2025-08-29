// Controller/State/ComposedStateBase.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using WPADevTools.Messaging;
using WPADevTools.SadExtensions.Common.Components;
using WPADevTools.SadExtensions.UI.Adornments;
using WPADevTools.SadExtensions.UI.Buttons;
using WPADevTools.SadExtensions.UI.Labels;
using WPADevTools.SadExtensions.UI.Layout;
using WPADevTools.SadExtensions.UI.Panels;
using SadConsole.Input;

namespace WPADevTools.Controller.State
{
    /// <summary>
    /// Composed state with chrome, menu console, a content host, and a toast host.
    /// Accepts a ComposedStateSpec to declare buttons and panels.
    /// </summary>
    public abstract class ComposedStateBase : AppStateBase
    {
        // Protected so derived states can use them
        protected ChromeSurface Chrome { get; }
        protected ControlsConsole MenuUi { get; }
        protected PanelHost ContentHost { get; }
        protected PanelHost ToastHost { get; }

        private readonly ComposedStateSpec _spec;
        private Button? _firstButton;

        protected ComposedStateBase(string name, ComposedStateSpec spec, Dimensions? chromeSize = null)
            : base(name)
        {
            _spec = spec;

            // chrome root
            Chrome = new ChromeSurface(chromeSize ?? new Dimensions(90, 30));
            Root.Children.Add(Chrome);

            // menu console (left/top area)
            MenuUi = new ControlsConsole(40, 12)
            {
                Position = new Point(3, 4),
                UseKeyboard = true,
                UseMouse = true,
                FocusOnMouseClick = true
            };
            Chrome.Children.Add(MenuUi);

            // content + toast hosts
            ContentHost = new PanelHost(PanelHostMode.Single);
            Chrome.Children.Add(ContentHost);

            ToastHost = new PanelHost(PanelHostMode.Multiple);
            Chrome.Children.Add(ToastHost); // last -> draws on top

            // Draw chrome
            new ChromeHeaderBox(_spec.Title).Draw(Chrome);
            new ChromeFooterBox(_spec.Footer).Draw(Chrome);

            // Build menu + optional initial panels
            BuildFromSpec();
        }

        public override void Update(TimeSpan delta)
        {
            var host = Controller.Instance.Host;
            if (host?.Keyboard.IsKeyReleased(Keys.Escape) == true)
            {
                // If a toast is showing, ESC dismisses that first.
                if (ToastHost.Children.Count > 0)
                {
                    WPADevTools.Messaging.EventHub.Publish(
                        WPADevTools.Messaging.AppMessage.ToastDismiss());
                    return;
                }

                // Otherwise, if this state says "back" is enabled, publish its intent.
                if (_spec.BackIntent is not null && (_spec.BackEnabled?.Invoke() ?? true))
                {
                    WPADevTools.Messaging.EventHub.Publish(_spec.BackIntent());
                }
            }
        }

        public override Task OnEnterAsync(Controller controller, CancellationToken ct)
        {
            MenuUi.IsVisible = true;
            MenuUi.IsEnabled = true;
            MenuUi.IsFocused = true;

            EventHub.Message += OnComposedAppMessage;
            return Task.CompletedTask;
        }

        public override Task OnExitAsync(Controller controller, CancellationToken ct)
        {
            EventHub.Message -= OnComposedAppMessage;

            // Clean UI
            ToastHost.Clear();
            ContentHost.Clear();
            return Task.CompletedTask;
        }

        private void BuildFromSpec()
        {
            MenuUi.Controls.Clear();

            // Title label
            MenuUi.Controls.Add(new TitleLabel(_spec.Title));

            // Buttons via stack
            var stack = new ButtonStack(MenuUi, x: 1, startY: 3, gapY: 2);
            var built = _spec.ButtonsFactory(stack)?.ToList() ?? [];
            if (built.Count > 0)
            {
                _firstButton = built[0];
                _firstButton.IsFocused = true;
                MenuUi.Controls.FocusedControl = _firstButton;
            }

            //// Hint under last button
            //var hintY = stack.NextY + 1;
            //MenuUi.Controls.Add(new HintLabel("Enter/Click to select • ESC: Back")
            //{
            //    Position = new Point(0, hintY)
            //});

            // Optional initial panels (rare)
            foreach (var p in _spec.PanelsFactory?.Invoke(ContentHost) ?? [])
                ContentHost.Add(p);
        }

        /// <summary>Convenience to update chrome header text.</summary>
        protected void SetHeader(string text) => ChromeHeaderBox.WriteText(Chrome, text);

        /// <summary>Show a centered confirm toast with Yes/No.</summary>
        protected void ShowConfirmToast(string message, Action onYes)
        {
            var toast = new ConfirmToast(new Point(0, 0), message);
            toast.CenterTo(Chrome);
            ToastHost.Clear();
            ToastHost.Add(toast);

            // Wire: the toast already publishes Quit/ToastDismiss via EventHub.
            // If you want a custom action instead of Quit, listen here:
            // (We keep it simple; derived can override OnComposedAppMessage if needed.)
        }

        private void OnComposedAppMessage(AppMessage msg)
        {
            // Default handling shared across states
            switch (msg.Type)
            {
                case AppMessageType.QuitRequested:
                    ShowConfirmToast("Really quit?", () => { /* handled by Controller on AppMessageType.Quit */ });
                    break;

                case AppMessageType.ToastDismiss:
                    ToastHost.Clear();
                    break;

                    // AppMessageType.Quit is handled by the Controller itself.
            }

            // Allow derived classes to extend handling
            HandleAppMessage(msg);
        }

        /// <summary>Derived states can extend message handling.</summary>
        protected virtual void HandleAppMessage(AppMessage msg) { }

        /// <summary>Restore focus to the first menu button, if present.</summary>
        protected void FocusFirstButton()
        {
            if (_firstButton != null)
            {
                _firstButton.IsFocused = true;
                MenuUi.Controls.FocusedControl = _firstButton;
            }
        }
    }
}
