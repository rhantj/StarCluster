using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class UpgradeJson
{
    public int upgrades;
    public List<ItemSlotJson> jsonSlots;
}

[Serializable]
public class ItemSlotJson
{
    public string itemName;
    public int count;
}

public static class UpgradeSaveLoad
{
    public static string path = Path.Combine(Application.persistentDataPath, "upgrade.json");

    public static void Save(UpgradeJson data)
    {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static bool TryLoadJson(out UpgradeJson data)
    {
        data = null;
        if (!File.Exists(path))
        {
            return false;
        }

        data = JsonUtility.FromJson<UpgradeJson>(File.ReadAllText(path));
        return data != null;
    }
}