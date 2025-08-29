using SadConsole.UI.Controls;

namespace WPADevTools.SadExtensions.UI.Buttons
{
    /// <summary>
    /// Button that auto-positions itself via a ButtonStack.
    /// </summary>
    public abstract class StackButton : Button
    {
        protected StackButton(ButtonStack stack, int width, string text) : base(width, 1)
        {
            Text = text;
            stack.Add(this);
        }
    }
}
