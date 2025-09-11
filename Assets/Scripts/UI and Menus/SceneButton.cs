using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    // Call this from the Button OnClick
    public void LoadScene()
    {
        if (Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            Debug.Log("Loading scene: " + sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene not found in Build Settings: " + sceneToLoad);
        }
    }
}

