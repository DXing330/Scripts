using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextList : MonoBehaviour
{
    public Color normal;
    public Color highlight;
    public List<string> allText;
    public void SetAllText(List<string> newTexts)
    {
        allText = newTexts;
        currentPage = 0;
        UpdateCurrentPage();
        DetermineChangeButtons();
    }
    public List<TMP_Text> textBoxes;
    public List<GameObject> textObjects;
    public int currentPage = 0;
    public List<GameObject> changeButtons;
    protected void DetermineChangeButtons()
    {
        if (allText.Count > textBoxes.Count)
        {
            GameManager.instance.utility.EnableAllObjects(changeButtons);
        }
        else
        {
            GameManager.instance.utility.DisableAllObjects(changeButtons);
        }
    }
    public void ChangePage(bool right = true)
    {
        int lastPage = allText.Count/textBoxes.Count;
        if (right)
        {
            if (currentPage < lastPage){currentPage++;}
            else{currentPage = 0;}
        }
        else
        {
            if (currentPage > 0){currentPage--;}
            else{currentPage = lastPage;}
        }
        UpdateCurrentPage();
    }

    protected void UpdateCurrentPage()
    {
        GameManager.instance.utility.DisableAllObjects(textObjects);
        int start = (currentPage*textBoxes.Count);
        int end = Mathf.Min(allText.Count, start + textBoxes.Count);
        for (int i = start; i < end; i++)
        {
            textObjects[i-start].SetActive(true);
            textBoxes[i-start].text = GameManager.instance.utility.SplitStringIntoLines(allText[i]);
        }
    }

    public void HighlightText(int index)
    {
        for (int i = 0; i < textBoxes.Count; i++)
        {
            textBoxes[i].color = normal;
        }
        if (index < 0){return;}
        textBoxes[index].color = highlight;
    }

    public string ReturnText(int index)
    {
        int realIndex = index + (currentPage*textBoxes.Count);
        return allText[realIndex];
    }
}
