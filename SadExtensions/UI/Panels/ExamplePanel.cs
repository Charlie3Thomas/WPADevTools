using SadRogue.Primitives;
using WPADevTools.SadExtensions.UI.Panels;

namespace WPADevTools.Controller.State.MainMenu.Components
{
    internal sealed class ExamplePanel : FramedPanel
    {
        private const string WorkingMessage = "Working… (auto-finish in ~2s)";
        private const string StatusMessage = "Done! Press ESC to return.";

        public ExamplePanel(int width, int height, Point position)
            : base(width, height, position)
        {
        }

        public void ShowWorking()
            => Print(2, 2, WorkingMessage, Color.White);

        public void PrintStatus(string text, Color color)
            => Print(2, 4, text, color);

        public async Task RunAsync(TimeSpan duration, CancellationToken ct)
        {
            var start = DateTime.UtcNow;

            while (DateTime.UtcNow - start < duration)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(50, ct);
            }

            PrintStatus(StatusMessage, Color.Green);
        }
    }
}
