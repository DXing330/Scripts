using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnchanterGUI : MarketPanelGUI
{
    void Start()
    {
        enchanterData = GameManager.instance.villageData.market.enchanterData;
        allEquipment = GameManager.instance.equipInventory;
        UpdateUser();
    }
    public SpriteContainer actorSprites;
    public SpriteContainer equipSprites;
    public EquipmentInventory allEquipment;
    public EnchanterDataManager enchanterData;
    public ScriptableDetailsViewer skillDetailsUtil;
    public ViewPassiveList currentPassives;
    public ViewPassiveList newPassives;
    public TMP_Text currentUserName;
    public StatImageText currentUserSprite;
    public StatImageText currentEnchanting;
    public int selectedUser = -1;
    public void ChangeUser(bool right = true)
    {
        selectedEquip = 0;
        selectedEnchantment = "";
        selectedUser = GameManager.instance.utility.ChangeIndex(selectedUser, right, allEquipment.allEquippedEquipment.Count, -1);
        UpdateUser();
        UpdateEquip();
    }
    protected void UpdateUser()
    {
        if (selectedUser == -1)
        {
            currentUserName.text = "";
            currentUserSprite.DisableSprite();
            possibleEquipment = new List<string>(allEquipment.allEquipment);
        }
        else
        {
            currentUserName.text = GameManager.instance.ReturnPartyMemberName(selectedUser);
            currentUserSprite.SetSprite(actorSprites.SpriteDictionary(GameManager.instance.ReturnPartyMemberType(selectedUser)));
            possibleEquipment = new List<string>(allEquipment.allEquippedEquipment[selectedUser].Split("@"));
            GameManager.instance.utility.RemoveEmptyListItems(possibleEquipment);
        }
        UpdateEquip();
    }
    public List<string> possibleEquipment;
    public List<string> equipData;
    public int selectedEquip = 0;
    public int selectedEquipType = 0;
    public int selectedCost = 0;
    public TMP_Text costText;
    protected void UpdateCost(int newCost)
    {
        selectedCost = newCost;
        costText.text = newCost.ToString();
    }
    public void ChangeEquip(bool right = true)
    {
        selectedEnchantment = "";
        selectedEquip = GameManager.instance.utility.ChangeIndex(selectedEquip, right, possibleEquipment.Count);
        UpdateEquip();
    }
    protected void UpdateEquip()
    {
        ResetEnchantments();
        if (possibleEquipment.Count <= 0)
        {
            currentEnchanting.DisableSprite();
            UpdateCost(0);
            return;
        }
        else
        {
            equipData = possibleEquipment[selectedEquip].Split("|").ToList();
            // Get the equip name.
            string equipName = equipData[^1];
            // Update the sprite.
            currentEnchanting.SetSprite(equipSprites.SpriteDictionary(equipName));
            // Get the equip type.
            selectedEquipType = Mathf.Max(0, int.Parse(equipData[6]));
            // Get the current equip passives.
            List<string> currentPassiveNames = equipData[5].Split("|").ToList();
            currentPassives.SetPassiveNames(currentPassiveNames);
            UpdateCost(GameManager.instance.utility.IntExponent(currentPassiveNames.Count));
        }
        UpdateEnchantments();
    }
    public List<string> possibleEnchantments;
    public string selectedEnchantment;
    protected void ResetEnchantments()
    {
        possibleEnchantments.Clear();
        newPassives.SetPassiveNames(possibleEnchantments);
        newPassives.ResetState();
    }
    protected void UpdateEnchantments()
    {
        // Get the possible enchantments.
        switch (selectedEquipType)
        {
            case 0:
                possibleEnchantments = new List<string>(enchanterData.weaponEnchantments);
                break;
            case 1:
                possibleEnchantments = new List<string>(enchanterData.armorEnchantments);
                break;
            case 2:
                possibleEnchantments = new List<string>(enchanterData.accessoryEnchantments);
                break;
        }
        // Remove any enchantments that are already on the weapon.
        newPassives.SetPassiveNames(possibleEnchantments.Except(equipData[5].Split("|").ToList()).ToList());
    }
    public void SelectEnchantment()
    {
        selectedEnchantment = newPassives.ReturnCurrentViewedPassiveName();
        if (selectedEnchantment == ""){return;}
        // Try to pay cost.
        // Determine what equipment to add it to.
        // Add the enchantment.
    }
}