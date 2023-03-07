using UnityEngine;
using Utility;

namespace Utility_Scripts
{
    public class CombineMesh : MonoBehaviour
    {
        // Attach this script to tile layers within the tilemap grid. It'll do separate operations
        // for each layer it is attached to.
        [SerializeField] private bool combineAtRuntime;
        private void Awake()
        {
            if (!combineAtRuntime) return;
        
            gameObject.MeshCombine();
        
        }
    }
}
