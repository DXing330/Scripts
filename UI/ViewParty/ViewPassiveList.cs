using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewPassiveList : MonoBehaviour
{
    void Start()
    {
        UpdateCurrentPage();
    }
    public ScriptableDetailsViewer passiveDetails;
    public List<string> passiveNames;
    public void SetPassiveNames(List<string> newNames)
    {
        passiveNames = newNames;
        currentPage = 0;
        UpdateCurrentPage();
    }
    public List<GameObject> listModeObjects;
    public List<GameObject> viewModeObjects;
    public List<GameObject> passiveButtons;
    public List<ButtonText> buttonTexts;
    public int currentPage = 0;
    public int currentViewed = -1;
    public int state = 0;
    public TMP_Text passiveName;
    public TMP_Text passiveEffect;

    public void SelectIndex(int index)
    {
        currentViewed = index + (currentPage*passiveButtons.Count);
        ChangeState();
    }

    protected void UpdateViewedPassive()
    {
        passiveName.text = passiveNames[currentViewed];
        passiveEffect.text = passiveDetails.ReturnSkillDescription(passiveNames[currentViewed]);
    }

    public void ChangeState()
    {
        state = (state+1)%2;
        if (state == 0)
        {
            GameManager.instance.utility.DisableAllObjects(viewModeObjects);
            GameManager.instance.utility.EnableAllObjects(listModeObjects);
            UpdateCurrentPage();
        }
        else
        {
            GameManager.instance.utility.DisableAllObjects(listModeObjects);
            GameManager.instance.utility.EnableAllObjects(viewModeObjects);
            UpdateViewedPassive();
        }
    }

    public void ChangePage(bool right = true)
    {
        if (state == 0)
        {
            currentPage = GameManager.instance.utility.ChangePage(currentPage, right, passiveButtons, passiveNames);
            UpdateCurrentPage();
        }
        else
        {
            currentViewed = GameManager.instance.utility.ChangeIndex(currentViewed, right, passiveNames.Count);
            UpdateViewedPassive();
        }
    }

    protected void UpdateCurrentPage()
    {
        GameManager.instance.utility.DisableAllObjects(passiveButtons);
        List<int> currentPageIndices = GameManager.instance.utility.GetNewPageIndices(currentPage, passiveButtons, passiveNames);
        for (int i = 0; i < currentPageIndices.Count; i++)
        {
            passiveButtons[i].SetActive(true);
            buttonTexts[i].UpdateButtonText(passiveNames[currentPageIndices[i]]);
        }
    }
}