using UnityEngine;

namespace Utility
{
    public class CombineMesh : MonoBehaviour
    {
        // Attach this script to tile layers within the tilemap grid. It'll do separate operations
        // for each layer it is attached to.
        [SerializeField] private bool combineAtRuntime = true;
        [SerializeField] private bool destroyObjects;
        // Sets the default ignore mask. Add "| 1 << xx" where xx is the layer integer afterwards to add more layers to it
        [SerializeField] private LayerMask combineIgnoreMask = 1 << 6 | 1 << 3;
        private void Awake()
        {
            if (!combineAtRuntime) return;
        
            gameObject.MeshCombine(combineIgnoreMask, destroyObjects);
        
        }
    }
}
