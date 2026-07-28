using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tether.Systems
{
    /// <summary>
    /// Guarantees the lighting + post-processing stack exists in every scene
    /// without hand-placing objects. Uses RuntimeInitializeOnLoadMethod so it
    /// runs before the first scene's Awake, and re-checks on each scene load.
    ///
    /// This exists because scene-level setup is easy to forget when adding a new
    /// scene, and a missing global light silently renders everything black.
    /// </summary>
    public static class VisualsBootstrap
    {
        private const string HostName = "~Visuals";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Ensure();
        }

        private static void OnSceneLoaded(Scene s, LoadSceneMode m) => Ensure();

        private static void Ensure()
        {
            // The bootstrap lives per-scene (not DontDestroyOnLoad) because the
            // runtime VolumeProfile and Light2D must belong to the scene whose
            // camera renders them.
            if (Object.FindFirstObjectByType<SceneLightingBootstrap>() != null) return;

            var go = new GameObject(HostName);
            go.AddComponent<SceneLightingBootstrap>();
            go.AddComponent<UI.ScreenFeedback>();
        }
    }
}
