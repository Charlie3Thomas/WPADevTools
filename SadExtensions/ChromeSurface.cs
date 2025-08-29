using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions
{
    public class ChromeSurface : ScreenSurface
    {
        public ChromeSurface(int width, int height)
            : base(width, height)
        {
            UseKeyboard = true;
            UseMouse = true;
            Surface.Fill(Color.Black, Color.Black, 0);
        }
    }
}
