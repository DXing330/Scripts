using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketPanelGUI : MonoBehaviour
{
    public GameObject panel;
    // List of equipment.
    public virtual void ActivatePanel()
    {
        panel.SetActive(true);
    }

    public virtual void DeactivatePanel(){panel.SetActive(false);}

    public virtual void ResetPanel(){}
}