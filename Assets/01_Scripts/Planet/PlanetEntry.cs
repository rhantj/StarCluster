using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.SceneManagement;

public enum PlanetStateMap
{
    Combat,
    Adventure
}

public class PlanetEntry : MonoBehaviour
{
    public AssetReference planetScene;
}
