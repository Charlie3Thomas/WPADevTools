// SadExtensions/UI/Panels/ConfirmToast.cs
using System;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using WPADevTools.Messaging;

namespace WPADevTools.SadExtensions.UI.Panels
{
    public sealed class ConfirmToast : FramedPanel
    {
        private readonly ControlsConsole _controls;
        private readonly int _innerW;
        private readonly int _innerH;

        public ConfirmToast(Point position, string message)
            : base(width: 36, height: 6, position: position)
        {
            // Inner client area (frame eats 1 cell border)
            _innerW = Surface.Width - 2;
            _innerH = Surface.Height - 2;

            // Controls host lives INSIDE the frame so borders stay visible
            _controls = new ControlsConsole(_innerW, _innerH)
            {
                Position = new Point(1, 1),
                UseMouse = true,
                UseKeyboard = true
            };
            Surface.Children.Add(_controls);

            // Message label at top of inner area
            var lbl = new Label(message ?? string.Empty)
            {
                Position = new Point(1, 0),
                TextColor = Color.White
            };
            _controls.Controls.Add(lbl);

            // Buttons one row above the inner bottom
            int row = _innerH - 2;
            var btnYes = new Button(10, 1) { Text = "Yes", Position = new Point((_innerW / 2) - 12, row) };
            var btnNo = new Button(10, 1) { Text = "No", Position = new Point((_innerW / 2) + 2, row) };

            btnYes.Click += (_, __) => EventHub.Publish(AppMessage.Quit());
            btnNo.Click += (_, __) => EventHub.Publish(AppMessage.ToastDismiss());

            _controls.Controls.Add(btnYes);
            _controls.Controls.Add(btnNo);

            _controls.IsFocused = true;
            btnNo.IsFocused = true; // safety default
        }
    }
}
