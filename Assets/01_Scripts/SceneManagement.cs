using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public bool isLoading { get; set; }
    public AssetReference scene;

    public void LoadScene()
    {
        isLoading = true;

        var load =
            Addressables.LoadSceneAsync(scene, LoadSceneMode.Single, activateOnLoad: false);

        load.Completed += OnSceneLoaded;
    }

    void OnSceneLoaded(AsyncOperationHandle<SceneInstance> scene)
    {
        if (scene.Status == AsyncOperationStatus.Succeeded)
        {
            var activate = scene.Result.ActivateAsync();
            activate.completed += e => isLoading = false;
        }
        else
        {
            Debug.LogError("Scene load fail");
            isLoading = false;
        }

        GameManager.Instance.CurrentSceneName = scene.Result.Scene.name;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlanetEntry>(out var p))
        {
            scene = p.planetScene;
            GameManager.Instance.SetPlanetState(p.pState);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlanetEntry>(out var p))
        {
            scene = null;
        }
    }
}
