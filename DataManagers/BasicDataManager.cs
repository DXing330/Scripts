using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDataManager : MonoBehaviour
{
    protected string saveDataPath;
    protected string loadedData;
    
    protected virtual void Start()
    {
        saveDataPath = Application.persistentDataPath;
    }
    
    public virtual void NewGame(){}

    public virtual void Load(){}

    public virtual void Save(){}
}
