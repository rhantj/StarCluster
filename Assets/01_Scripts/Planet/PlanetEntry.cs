using System;
using System.Collections.Generic;
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
    public ItemData[] itemDatas;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.SetPlanetItemData(itemDatas);
    }
}
