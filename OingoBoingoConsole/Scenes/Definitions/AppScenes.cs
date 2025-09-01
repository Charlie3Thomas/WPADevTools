namespace OingoBoingoConsole.Scenes.Definitions
{
    /// <summary>
    /// Central catalog for all scenes in the app.
    /// </summary>
    public static partial class AppScenes
    {
        /// <summary>
        /// The scene you want to start on.
        /// </summary>
        public static SceneKey Initial => SceneKey.Home;

        /// <summary>
        /// Builds the full scene dictionary (keyed by SceneKey).
        /// </summary>
        public static Dictionary<SceneKey, SceneDefinition> Build()
        {
            return SceneDefs.Map(
                Home,
                Donut,
                DemoA,
                DemoB
            );
        }
    }
}
