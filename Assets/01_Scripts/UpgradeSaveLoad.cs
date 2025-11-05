using System.IO;
using UnityEngine;

public static class UpgradeSaveLoad
{
    public static string path = Path.Combine(Application.persistentDataPath, "upgrade.json");

    public static void Save(int upgrade)
    {
        var data = new UpgradeJson
        {
            upgrades = upgrade
        };

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static bool TryLoadJson(out int upgrade)
    {
        if (!File.Exists(path))
        {
            upgrade = 0;
            return false;
        }

        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<UpgradeJson>(json);

        upgrade = data.upgrades;
        return true;
    }
}