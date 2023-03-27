using UnityEngine;

namespace Utility {

    public class ItemController : MonoBehaviour
    {

        [SerializeField]
        private int _value = 1;

        public int GetValue()
        {
            return _value;
        }

    }
}
