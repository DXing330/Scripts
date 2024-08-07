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

    public void EnableSelectedObject(List<GameObject> objects, int index = 0)
    {
        DisableAllObjects(objects);
        if (index >= 0 && index < objects.Count){objects[index].SetActive(true);}
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

    public int FloorSqrt(int number)
    {
        int sqrt = 1;
        if (number <= 1){return sqrt;}
        for (int i = 0; i < number/2; i++)
        {
            sqrt++;
            if (sqrt*sqrt > number){return (sqrt-1);}
        }
        return sqrt;
    }

    public int FloorLog(int number, int BASE = 2)
    {
        if (number < BASE){return 1;}
        int log = 1;
        for (int i = 0; i < number/BASE; i++)
        {
            if (number < BASE){break;}
            number /= BASE;
            log++;
        }
        return log;
    }

    public int IntExponent(int expPow, int expBase = 2)
    {
        if (expPow == 0){return 1;}
        int value = 1;
        for (int i = 0; i < expPow; i++)
        {
            value *= expBase;
        }
        return value;
    }
    
    public string ConvertListToString(List<string> string_list, string delimiter = "|")
    {
        return String.Join(delimiter, string_list);
    }

    public List<string> SplitStringIntoGroups(string input, int length, string delimiter = "|")
    {
        List<string> string_list = new List<string>();
        string[] string_array = input.Split(delimiter);
        string new_string = "";
        int index = 0;
        for (int i = 0; i < string_array.Length/length; i++)
        {
            new_string = "";
            for (int j = 0; j < length; j++)
            {
                new_string += string_array[index]+"|";
                index++;
            }
            string_list.Add(new_string);
        }
        return string_list;
    }

    public string ConvertIntListToString(List<int> int_list, string delimiter = "|")
    {
        List<string> string_list = new List<string>();
        for (int i = 0; i < int_list.Count; i++)
        {
            string_list.Add(int_list[i].ToString());
        }
        return ConvertListToString(string_list, delimiter);
    }

    public List<int> StringListToIntList(List<string> string_list)
    {
        List<int> int_list = new List<int>();
        for (int i = 0; i < string_list.Count; i++)
        {
            int_list.Add(int.Parse(string_list[i]));
        }
        return int_list;
    }
    
    public void RemoveEmptyListItems(List<string> listToRemoveFrom, int minLength = 1)
    {
        for (int i = listToRemoveFrom.Count - 1; i > -1; i--)
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

    public List<TacticActor> QuickSortActorsByIntList(List<TacticActor> actors, List<int> stats, int leftIndex, int rightIndex)
    {
        int i = leftIndex;
        int j = rightIndex;
        int pivotStat = stats[leftIndex];
        while (i <= j)
        {
            while (stats[i] > pivotStat)
            {
                i++;
            }
            while (stats[j] < pivotStat)
            {
                j--;
            }
            if (i <= j)
            {
                int temp = stats[i];
                stats[i] = stats[j];
                stats[j] = temp;
                TacticActor tempActor = actors[i];
                actors[i] = actors[j];
                actors[j] = tempActor;
                i++;
                j--;
            }
        }
        if (leftIndex < j)
        {
            QuickSortActorsByIntList(actors, stats, leftIndex, j);
        }
        if (i < rightIndex)
        {
            QuickSortActorsByIntList(actors, stats, i, rightIndex);
        }
        return actors;
    }

    public int ChangePage(int currentPage, bool right, List<GameObject> pageLength, List<string> dataList)
    {
        int maxPage = dataList.Count/pageLength.Count;
        if (dataList.Count%pageLength.Count == 0){maxPage--;}
        if (right)
        {
            if (currentPage < maxPage){currentPage++;}
            else{currentPage = 0;}
        }
        else
        {
            if (currentPage > 0){currentPage--;}
            else{currentPage = maxPage;}
        }
        return currentPage;
    }

    public int ChangeIndex(int currentIndex, bool right, int length, int minIndex = 0)
    {
        if (right)
        {
            if (currentIndex < length - 1){currentIndex++;}
            else{currentIndex = minIndex;}
        }
        else
        {
            if (currentIndex > minIndex){currentIndex--;}
            else{currentIndex = length - 1;}
        }
        return currentIndex;
    }
    
    public List<int> GetNewPageIndices(int currentPage, List<GameObject> pageLength, List<string> dataList)
    {
        List<int> newPageIndices = new List<int>();
        int startIndex = currentPage*pageLength.Count;
        for (int i = 0; i < Mathf.Min(pageLength.Count, dataList.Count - startIndex); i++)
        {
            newPageIndices.Add(startIndex + i);
        }
        return newPageIndices;
    }

    public int GetRow(int location, int size)
    {
        int row = 0;
        while (location >= size)
        {
            location -= size;
            row++;
        }
        return row;
    }

    public int GetColumn(int location, int size)
    {
        return location%size;
    }

    public int GetHexQ(int location, int size)
    {
        return GetColumn(location, size);
    }

    public int GetHexR(int location, int size)
    {
        return GetRow(location, size) - (GetColumn(location, size) - (GetColumn(location, size)%2)) / 2;
    }

    public int GetHexS(int location, int size)
    {
        return -GetHexQ(location, size)-GetHexR(location, size);
    }

    public int PointInDirection(int location, int direction, int size)
    {
        switch (direction)
        {
            // Up.
            case 0:
                return location - size;
            // UpRight.
            case 1:
                if (GetColumn(location, size)%2 == 1)
                {
                    return location + 1;
                }
                return (location - size + 1);
            // DownRight.
            case 2:
                if (GetColumn(location, size)%2 == 0)
                {
                    return location + 1;
                }
                return (location + size + 1);
            // Down.
            case 3:
                return location + size;
            // DownLeft.
            case 4:
                if (GetColumn(location, size)%2 == 0)
                {
                    return location - 1;
                }
                return (location + size - 1);
            // UpLeft.
            case 5:
                if (GetColumn(location, size)%2 == 1)
                {
                    return location - 1;
                }
                return (location - size - 1);
        }
        return location;
    }    

    public int DistanceBetweenPoints(int p1, int p2, int size)
    {
        int q1 = GetHexQ(p1, size);
        int r1 = GetHexR(p1, size);
        int q2 = GetHexQ(p2, size);
        int r2 = GetHexR(p2, size);
        int distance = (Mathf.Abs(q1-q2) + Mathf.Abs(q1+r1-q2-r2) + Mathf.Abs(r1-r2))/2;
        return distance;
    }
}
