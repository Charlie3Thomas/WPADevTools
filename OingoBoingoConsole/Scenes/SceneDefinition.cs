// ===== File: Scenes/SceneDefinition.cs =====
using OingoBoingoConsole.ScreenExtensions;

namespace OingoBoingoConsole.Scenes
{
    public sealed record class SceneDefinition
    {
        public required SceneKey Key { get; init; }
        public required string Title { get; init; }
        public required Func<CustomScreen> Factory { get; init; }

        public SceneKey? ParentKey { get; init; }
        public SceneKey[]? ChildrenKeys { get; init; }
    }

    public static class SceneDefs
    {
        public static Dictionary<SceneKey, SceneDefinition> Map(params SceneDefinition[] defs)
            => defs.ToDictionary(d => d.Key);
    }
}
