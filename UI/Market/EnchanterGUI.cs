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
    public TMP_Text newEnchantmentName;
    public TMP_Text newEnchantmentEffect;
    public ViewPassiveList currentPassives;
    public TMP_Text currentUserName;
    public StatImageText currentUserSprite;
    public StatImageText currentEnchanting;
    public int selectedUser = -1;
    public void ChangeUser(bool right = true)
    {
        selectedEquip = 0;
        selectedEnchantment = 0;
        selectedUser = GameManager.instance.utility.ChangeIndex(selectedUser, right, allEquipment.allEquippedEquipment.Count, -1);
        UpdateUser();
    }
    protected void UpdateUser()
    {
        if (selectedUser == -1)
        {
            currentUserName.text = "";
            currentUserSprite.DisableSprite();
            // Get the unequipped equipment.
        }
        else
        {
            currentUserName.text = GameManager.instance.ReturnPartyMemberName(selectedUser);
            currentUserSprite.SetSprite(actorSprites.SpriteDictionary(GameManager.instance.ReturnPartyMemberType(selectedUser)));
            // Get the equipped equipment of that user.
        }
    }
    public List<string> possibleEquipment;
    public int selectedEquip = 0;
    public int selectedEquipType = 0;
    public void ChangeEquip(bool right = true)
    {
        selectedEnchantment = 0;
        // TODO, change to the next equipment. Do we need to unequip the equipment first? Can pick the user of the equipment when deciding what to enchant
    }
    public int selectedEnchantment = 0;
    

    protected void ResetNewEnchantment()
    {
        newEnchantmentName.text = "None";
        newEnchantmentEffect.text = "There are currently no enchantments available for this equipment.";
    }

    protected void UpdateEnchantment()
    {

    }
}