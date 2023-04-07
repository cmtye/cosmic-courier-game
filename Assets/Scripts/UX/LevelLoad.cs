using UnityEngine;
using UnityEngine.SceneManagement;

namespace UX
{
    public class LevelLoad : MonoBehaviour
    {

        public int SceneIndex;

        public void Load()
        {
            SceneManager.LoadScene(SceneIndex);
        }

    }
}
