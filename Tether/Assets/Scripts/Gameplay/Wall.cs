using UnityEngine;

namespace Tether.Gameplay
{
    /// <summary>
    /// Phase 1 arena wall. Passive collider — Rigidbody2D physics handles bounce automatically.
    /// Marker component for design intent; extend later with wall types (bouncy, damaging, etc).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Wall : MonoBehaviour
    {
        // Empty marker for now. Presence of this component identifies "wall" in the arena.
    }
}
