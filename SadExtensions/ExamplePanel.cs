using SadConsole;
using SadRogue.Primitives;

namespace WPADevTools.SadExtensions
{
    internal sealed class ExamplePanel
        : IDisposable
    {
        public ScreenSurface Panel { get; }

        private bool _attached;

        public ExamplePanel(int width, int height, Point position)
        {
            Panel = new ScreenSurface(width, height)
            {
                Position = position
            };

            Panel.Surface.Fill(Color.Black, Color.Black, 0);
            Panel.DrawBox(
                new Rectangle(0, 0, width, height),
                ShapeParameters.CreateStyledBox(
                    ICellSurface.ConnectedLineThin,
                    new ColoredGlyph(Color.White, Color.Black)));
        }

        public void AttachTo(ScreenObject parent)
        {
            if (_attached) return;
            parent.Children.Add(Panel);
            _attached = true;
        }

        public void DetachFrom(ScreenObject parent)
        {
            if (!_attached) return;
            parent.Children.Remove(Panel);
            _attached = false;
        }

        public void ShowWorking(string message = "Working… (auto-finish in ~2s)")
        {
            Panel.Surface.Print(2, 2, message, Color.White);
        }

        public void PrintStatus(string text, Color color)
        {
            Panel.Surface.Print(2, 4, text, color);
        }

        public async Task RunAsync(TimeSpan duration, CancellationToken ct)
        {
            var start = DateTime.UtcNow;
            while (DateTime.UtcNow - start < duration)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(50, ct);
            }

            PrintStatus("Done! Press ESC to return.", Color.Green);
        }

        public void Dispose()
        {
            Panel.Dispose();
        }
    }
}
