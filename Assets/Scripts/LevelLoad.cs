using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour
{

    public int SceneIndex;

    public void Load()
    {
        SceneManager.LoadScene(SceneIndex);
    }

}
