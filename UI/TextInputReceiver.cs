using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextInputReceiver : MonoBehaviour
{
    public TMP_InputField input;
    public TMP_Text text;
    public int charLimit = 13;
    public int selectedName = 0;

    void Start()
    {
        text.text = "";
        for (int i = 0; i < charLimit; i++)
        {
            text.text += "_";
        }
    }

    public void UpdateText()
    {
        string newText = input.text;
        if (newText.Length > charLimit){return;}
        text.text = "";
        for (int i = 0; i < charLimit; i++)
        {
            if (i < newText.Length){text.text += newText[i];}
            else {text.text += "_";}
        }
    }

    protected string ReturnText()
    {
        return text.text;
    }

    public void FinishSelectingName()
    {
        if (input.text.Length < 1){return;}
        GameManager.instance.UpdatePlayerName(input.text);
        GameManager.instance.ReturnToHub();
    }
}
