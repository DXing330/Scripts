using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDataManager : MonoBehaviour
{
    public string fileName;
    public string newGameData;
    protected string saveDataPath;
    protected string loadedData;
    
    protected virtual void Start()
    {
        saveDataPath = Application.persistentDataPath;
    }
    
    [ContextMenu("New Game")]
    public virtual void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            File.Delete (saveDataPath+fileName);
        }
        File.WriteAllText(saveDataPath+fileName, newGameData);
        Load();
    }

    public virtual void Load(){}

    public virtual void Save(){}
}
