#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tether.EditorTools
{
    /// <summary>
    /// Editor-only: when Play is pressed, always start from MainMenu no matter
    /// which scene is currently open in the editor. When Play ends, Unity
    /// restores the previously-open scene automatically.
    /// </summary>
    [InitializeOnLoad]
    public static class PlayFromMainMenu
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";

        static PlayFromMainMenu()
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(MainMenuScenePath);
            if (sceneAsset != null)
            {
                EditorSceneManager.playModeStartScene = sceneAsset;
            }
        }
    }
}
#endif
