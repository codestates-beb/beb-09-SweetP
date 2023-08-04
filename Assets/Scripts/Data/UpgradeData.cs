using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class UpgradeData : MonoBehaviour
{
    public TextAsset data;
    private AllData datas;

    // Start is called before the first frame update
    private void Awake()
    {
        datas = JsonUtility.FromJson<AllData>(data.text);
        UpgradeSheetDictionary = new Dictionary<int, UpgradeSheet>();

        // Populating the UpgradeSheetDictionary for easy access by upgrade value
        foreach (UpgradeSheet upgradeSheet in datas.weapon)
        {
            UpgradeSheetDictionary[upgradeSheet.upgrade] = upgradeSheet;
        }
    }

    // Dictionary to hold UpgradeSheet objects with upgrade as key
    private Dictionary<int, UpgradeSheet> UpgradeSheetDictionary;

    // Method to get prob value for a specific upgrade
    public int GetProbForUpgrade(int upgrade)
    {
        if (UpgradeSheetDictionary.TryGetValue(upgrade, out var upgradeSheet))
        {
            return upgradeSheet.prob;
        }
        else
        {
            // Handle case when upgrade value is not found
            Debug.LogError("Upgrade value not found: " + upgrade);
            return 0; // Or return a default value as per your requirement
        }
    }

    // Method to get cost value for a specific upgrade
    public int GetCostForUpgrade(int upgrade)
    {
        if (UpgradeSheetDictionary.TryGetValue(upgrade, out var upgradeSheet))
        {
            return upgradeSheet.cost;
        }
        else
        {
            // Handle case when upgrade value is not found
            Debug.LogError("Upgrade value not found: " + upgrade);
            return 0; // Or return a default value as per your requirement
        }
    }
}

[System.Serializable]
public class AllData
{
    public UpgradeSheet[] weapon;
}

[System.Serializable]
public class UpgradeSheet
{
    public int upgrade;
    public int prob;
    public int cost;
}