using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{
    [SerializeField] private string[] scenesToLoad = { "AlienForest_Environment" };

    private void Awake()
    {
        foreach (string sceneName in scenesToLoad)
        {
            if (string.IsNullOrEmpty(sceneName))
                continue;

            if (SceneAlreadyPresent(sceneName))
                continue;

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    private static bool SceneAlreadyPresent(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
                return true;
        }
        return false;
    }
}
