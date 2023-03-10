using UnityEngine;
using Utility;

namespace Utility_Scripts
{
    public class CombineMesh : MonoBehaviour
    {
        // Attach this script to tile layers within the tilemap grid. It'll do separate operations
        // for each layer it is attached to.
        [SerializeField] private bool combineAtRuntime = true;
        // Sets the default ignore mask to layer 6. Add "| 1 << xx" afterwards to add more layers to it
        [SerializeField] private LayerMask combineIgnoreMask = 1 << 6;
        private void Awake()
        {
            if (!combineAtRuntime) return;
        
            gameObject.MeshCombine(combineIgnoreMask);
        
        }
    }
}
