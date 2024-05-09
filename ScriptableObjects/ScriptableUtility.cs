using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Utility", menuName = "ScriptableObjects/Utility", order = 1)]
public class ScriptableUtility : ScriptableObject
{
    public void DataManagerSave(List<BasicDataManager> data)
    {
        foreach (BasicDataManager dataManager in data)
        {
            dataManager.Save();
        }
    }

    public void DataManagerLoad(List<BasicDataManager> data)
    {
        foreach (BasicDataManager dataManager in data)
        {
            dataManager.Load();
        }
    }

    public void DataManagerNewGame(List<BasicDataManager> data)
    {
        foreach (BasicDataManager dataManager in data)
        {
            dataManager.NewGame();
        }
    }

    public void DisableAllObjects(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(false);
        }
    }

    public void EnableAllObjects(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(true);
        }
    }

    public bool DivisibleNumber(int input, int check = 2)
    {
        return (input%check==0);
    }

    public bool EvenNumber(int number)
    {
        return (number%2==0);
    }

    public bool DivisibleThree(int number)
    {
        return (number%3==0);
    }

    public string ConvertListToString(List<string> string_list, string delimiter = "|")
    {
        return String.Join(delimiter, string_list);
    }

    public void RemoveEmptyListItems(List<string> listToRemoveFrom, int minLength = 1)
    {
        for (int i = 0; i < listToRemoveFrom.Count; i++)
        {
            if (listToRemoveFrom[i].Length <= minLength)
            {
                listToRemoveFrom.RemoveAt(i);
            }
        }
    }

    public int CountOccurencesOfStringInList(List<string> listToCountFrom, string stringToCount)
    {
        int count = listToCountFrom.Count(s => s == stringToCount);
        return count;
    }

    public int CountOccurencesOfCharInString(string stringToCountFrom, char charToCount)
    {
        int count = 0;
        for (int i = 0; i < stringToCountFrom.Length; i++)
        {
            if (stringToCountFrom[i] == charToCount)
            {
                count++;
            }
        }
        return count;
    }

    public List<int> QuickSortIntList(List<int> intList, int leftIndex, int rightIndex)
    {
        int i = leftIndex;
        int j = rightIndex;
        int pivot = intList[leftIndex];
        while (i <= j)
        {
            while (intList[i] > pivot)
            {
                i++;
            }
            while (intList[j] < pivot)
            {
                j--;
            }
            if (i <= j)
            {
                int temp = intList[i];
                intList[i] = intList[j];
                intList[j] = temp;
                i++;
                j--;
            }
        }
        if (leftIndex < j)
        {
            QuickSortIntList(intList, leftIndex, j);
        }
        if (i < rightIndex)
        {
            QuickSortIntList(intList, i, rightIndex);
        }
        return intList;
    }

    public List<string> QuickSortListbyIntList(List<string> stringList, List<int> intList, int leftIndex, int rightIndex)
    {
        int i = leftIndex;
        int j = rightIndex;
        int pivot = intList[leftIndex];
        while (i <= j)
        {
            while (intList[i] > pivot)
            {
                i++;
            }
            while (intList[j] < pivot)
            {
                j--;
            }
            if (i <= j)
            {
                int temp = intList[i];
                intList[i] = intList[j];
                intList[j] = temp;
                string tempString = stringList[i];
                stringList[i] = stringList[j];
                stringList[j] = tempString;
                i++;
                j--;
            }
        }
        if (leftIndex < j)
        {
            QuickSortListbyIntList(stringList, intList, leftIndex, j);
        }
        if (i < rightIndex)
        {
            QuickSortListbyIntList(stringList, intList, i, rightIndex);
        }
        return stringList;
    }

    public List<string> QuickSortListByStringIntList(List<string> stringList, List<string> intList, int leftIndex, int rightIndex)
    {
        int i = leftIndex;
        int j = rightIndex;
        int pivot = int.Parse(intList[leftIndex]);
        while (i <= j)
        {
            while (int.Parse(intList[i]) > pivot)
            {
                i++;
            }
            while (int.Parse(intList[j]) < pivot)
            {
                j--;
            }
            if (i <= j)
            {
                string temp = intList[i];
                intList[i] = intList[j];
                intList[j] = temp;
                string tempString = stringList[i];
                stringList[i] = stringList[j];
                stringList[j] = tempString;
                i++;
                j--;
            }
        }
        if (leftIndex < j)
        {
            QuickSortListByStringIntList(stringList, intList, leftIndex, j);
        }
        if (i < rightIndex)
        {
            QuickSortListByStringIntList(stringList, intList, i, rightIndex);
        }
        return stringList;
    }

    public string SplitStringIntoLines(string oString, string delimiter = " ")
    {
        string newString = "";
        string[] parts = oString.Split(delimiter);
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length < 1){continue;}
            newString += parts[i];
            if (i <= parts.Length - 1){newString += "\n";}
        }
        return newString;
    }

}
