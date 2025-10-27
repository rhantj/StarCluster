using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/Item Data")]

public class ItemData : ScriptableObject
{
    public string Name;
    public int Count;
    public Sprite Icon;
}
