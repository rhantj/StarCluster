using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Object/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public List<ItemData> itemDatas = new();
    public List<int> itemCounts = new();
}
