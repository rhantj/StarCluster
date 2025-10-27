using UnityEngine;
using UnityEngine.AddressableAssets;

public enum PlanetStateMap
{
    Combat,
    Adventure
}

public class PlanetEntry : MonoBehaviour
{
    public AssetReference planetScene;
    public PlanetStateMap pState;
}
