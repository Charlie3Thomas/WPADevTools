// ===== File: Scenes/SceneRouter.cs =====
using OingoBoingoConsole.ScreenExtensions;
using SadConsole;
using SadConsole.Instructions;
using SadConsole.Renderers;

namespace OingoBoingoConsole.Scenes
{
    public sealed class SceneRouter
    {
        private readonly Dictionary<SceneKey, SceneDefinition> _defs;
        private readonly ScreenObject _host;   // where scenes get attached

        private readonly List<SceneKey> _history = new();
        private int _index = -1;

        public SceneRouter(Dictionary<SceneKey, SceneDefinition> defs, ScreenObject host)
        {
            _defs = defs;
            _host = host;
        }

        public CustomScreen? Current { get; private set; }

        public bool CanBack => _index > 0;
        public bool CanForward => _index >= 0 && _index + 1 < _history.Count;

        public event Action<CustomScreen, SceneDefinition>? SceneChanged;

        public void NavigateTo(SceneKey key, bool replace = false, bool fade = true)
        {
            if (!_defs.TryGetValue(key, out var def)) return;

            var next = def.Factory();

            // Auto “< Back” button if this scene has a parent
            if (def.ParentKey is SceneKey parent)
                next.AddSceneBackButton(parent, this);

            if (Current != null && fade)
                CrossFade(Current, next);
            else
                ReplaceCurrent(next);

            if (replace && _index >= 0) _history[_index] = key;
            else
            {
                if (_index < _history.Count - 1)
                    _history.RemoveRange(_index + 1, _history.Count - (_index + 1));
                _history.Add(key);
                _index = _history.Count - 1;
            }

            SceneChanged?.Invoke(next, def);
        }

        public void Back()
        {
            if (!CanBack) return;
            _index--;
            NavigateTo(_history[_index], replace: true, fade: true);
        }

        public void Forward()
        {
            if (!CanForward) return;
            _index++;
            NavigateTo(_history[_index], replace: true, fade: true);
        }

        private void ReplaceCurrent(CustomScreen scene)
        {
            if (Current?.Parent != null) _host.Children.Remove(Current);
            _host.Children.Add(scene);
            Current = scene;
        }

        private static IScreenSurface? FindFirstSurface(IScreenObject node)
        {
            if (node is IScreenSurface s) return s;

            foreach (var child in node.Children)
            {
                var found = FindFirstSurface(child);
                if (found != null) return found;
            }

            return null;
        }

        private void CrossFade(CustomScreen oldScene, CustomScreen newScene)
        {
            _host.Children.Add(newScene);

            var fromSurf = FindFirstSurface(oldScene);
            var toSurf = FindFirstSurface(newScene);

            var fromR = fromSurf?.Renderer as ScreenSurfaceRenderer;
            var toR = toSurf?.Renderer as ScreenSurfaceRenderer;

            if (fromR == null || toR == null)
            {
                _host.Children.Remove(oldScene);
                Current = newScene;
                return;
            }

            toR.Opacity = 0;

            var step = new AnimatedValue(
                TimeSpan.FromMilliseconds(350),
                0, 255,
                new SadConsole.EasingFunctions.Linear { Mode = SadConsole.EasingFunctions.EasingMode.InOut });

            step.ValueChanged += (_, value) =>
            {
                byte b = (byte)value;
                byte a = (byte)(255 - b);
                fromR.Opacity = a;
                toR.Opacity = b;
            };

            step.Finished += (_, __) =>
            {
                toR.Opacity = 255;
                if (oldScene.Parent != null) _host.Children.Remove(oldScene);
                Current = newScene;
            };

            step.RemoveOnFinished = true;
            _host.SadComponents.Add(step);
        }
    }
}
