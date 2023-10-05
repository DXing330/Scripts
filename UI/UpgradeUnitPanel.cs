using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUnitPanel : MonoBehaviour
{
    public string unitName;
    public int upgradeCostType;
    public int upgradeCostAmount;
    public Text title;
    public Text healthUpgrade;
    public Text attackUpgrade;
    public Text defenseUpgrade;

    void Start()
    {
        UpdateUpgradeInfo();
    }

    private void ResetUpgradeInfo()
    {
        healthUpgrade.text = "0 %";
        attackUpgrade.text = "0 %";
        defenseUpgrade.text = "0 %";
    }

    public void UpdateUpgradeInfo()
    {
        string upgradeInfo = GameManager.instance.upgradeData.ReturnUpgradeAmounts(unitName);
        if (upgradeInfo.Length < 5)
        {
            ResetUpgradeInfo();
            return;
        }
        string[] upgradeAmounts = upgradeInfo.Split("|");
        healthUpgrade.text = upgradeAmounts[0]+" %";
        attackUpgrade.text = upgradeAmounts[1]+" %";
        defenseUpgrade.text = upgradeAmounts[2]+" %";
    }

    public void UpgradeUnit()
    {
        if (GameManager.instance.CheckCost(upgradeCostType, upgradeCostAmount))
        {
            GameManager.instance.GainResource(upgradeCostType, -upgradeCostAmount);
            GameManager.instance.upgradeData.UpgradeUnitRandomly(unitName);
        }
    }
}
