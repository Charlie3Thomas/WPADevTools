using SadConsole.UI.Controls;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    public abstract class StackButton
        : Button
    {
        protected StackButton(ButtonStack stack, int width, string text)
            : base(width, 1)
        {
            Text = text;
            stack.Add(this);
        }
    }
}
