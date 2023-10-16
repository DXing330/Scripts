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
        string unitInfo = GameManager.instance.actorData.ReturnActorBaseStats(unitName);
        string[] unitStats = unitInfo.Split("|");
        if (upgradeInfo.Length < 5)
        {
            healthUpgrade.text = unitStats[0];
            attackUpgrade.text = unitStats[1];
            defenseUpgrade.text = unitStats[2];
            return;
        }
        string[] upgradeAmounts = upgradeInfo.Split("|");
        healthUpgrade.text = (int.Parse(unitStats[0])+(int.Parse(unitStats[0])*int.Parse(upgradeAmounts[0])/100)).ToString();
        attackUpgrade.text = (int.Parse(unitStats[1])+(int.Parse(unitStats[1])*int.Parse(upgradeAmounts[1])/100)).ToString();
        defenseUpgrade.text = (int.Parse(unitStats[2])+(int.Parse(unitStats[2])*int.Parse(upgradeAmounts[2])/100)).ToString();
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
