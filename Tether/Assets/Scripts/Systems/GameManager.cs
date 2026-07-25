using UnityEngine;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 1 baseline GameManager. Singleton entry point for game-wide state.
    /// Extended later with score/wave state, run tracking, meta progression.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private bool _persistAcrossScenes = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (_persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
