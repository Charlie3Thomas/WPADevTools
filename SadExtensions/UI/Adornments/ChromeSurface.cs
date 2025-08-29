using SadConsole;
using SadRogue.Primitives;
using WPADevTools.SadExtensions.Common.Components;

namespace WPADevTools.SadExtensions.UI.Adornments
{
    public class ChromeSurface
        : ScreenSurface
    {
        public ChromeSurface(Dimensions dimensions)
            : base(dimensions.Width, dimensions.Height)
        {
            UseKeyboard = true;
            UseMouse = true;
            Surface.Fill(Color.Black, Color.Black, 0);
        }
    }
}
