using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGen", menuName = "ScriptableObjects/DungeonGen", order = 1)]
public class DungeonGenerator : ScriptableObject
{
    public ScriptableUtility utility;
    public int size;
    protected int minSize = 36;
    public int GetMinSize(){return minSize;}
    public int minRoomSize = 6;
    public int maxRooms = 6;
    public List<int> allTiles = new List<int>();
    public List<string> roomDetails = new List<string>();
    //public List<int> impassableTiles = new List<int>();
    public List<string> GenerateDungeon(int newSize = -1, int newMaxRooms = -1)
    {
        size = Mathf.Max(minSize, newSize);
        maxRooms = Mathf.Max(size/minRoomSize, newMaxRooms);
        Reset();
        List<string> dungeonData = new List<string>();
        // Get base size.
        GetTiles();
        // Make a maze.
        MakeRooms();
        // Get a starting point.
            // Inside a randomly selected room.
        int start = GetRandomPointInRoom(Random.Range(0, roomDetails.Count));
        // Get an exit.
            // Inside a randomly selected room.
        int end = GetRandomPointInRoom(Random.Range(0, roomDetails.Count));
        // Get treasure locations.
            // Inside 1+ randomly selected room(s).
        int treasure = GetRandomPointInRoom(Random.Range(0, roomDetails.Count), end);
        // Get enemies.
            // Starting amount equal to size or sqrt(size)?
            // Enemies moves every turn and spawn every X turns.
        Debug.Log(utility.ConvertListToString(utility.SplitStringIntoGroups(utility.ConvertIntListToString(allTiles), size), "#"));
        dungeonData.Add(utility.ConvertIntListToString(allTiles));
        dungeonData.Add(start.ToString());
        dungeonData.Add(end.ToString());
        dungeonData.Add(treasure.ToString());
        return dungeonData;
    }

    protected int GetRandomPointInRoom(int roomNum, int except = -1)
    {
        List<int> roomTiles = GetRoomTiles(roomDetails[roomNum]);
        int start = roomTiles[Random.Range(0, roomTiles.Count)];
        if (start == except){return GetRandomPointInRoom(roomNum, except);}
        allTiles[start] = 0;
        return start;
    }

    protected void Reset()
    {
        allTiles.Clear();
    }

    protected void GetTiles()
    {
        for (int i = 0; i < size * size; i++)
        {
            allTiles.Add(1);
        }
    }

    protected void MakeRooms(int tries = 100)
    {
        roomDetails.Clear();
        for (int i = 0; i < tries; i++)
        {
            TryToMakeRoom();
            if (roomDetails.Count >= maxRooms){break;}
        }
        if (roomDetails.Count <= 1)
        {
            for (int i = 0; i < size*size; i++)
            {
                allTiles[i] = 0;
            }
        }
        // Otherwise try to connect all the room.
        else
        {
            for (int i = 1; i < roomDetails.Count; i++)
            {
                ConnectRooms(0, i);
                ConnectRooms(Random.Range(0, roomDetails.Count), Random.Range(0, roomDetails.Count));
            }
        }
    }

    protected void ConnectPoints(int startPoint, int endPoint)
    {
        int startRow = (startPoint/size);
        int startCol = GetColumn(startPoint);
        int endRow = (endPoint/size);
        int endCol = GetColumn(endPoint);
        List<int> possibleNextPoints = new List<int>();
        for (int i = 0; i < size*size; i++)
        {
            possibleNextPoints.Clear();
            startRow = (startPoint/size);
            startCol = GetColumn(startPoint);
            // Randomly path in the general direction of the end point.
            if (startRow < endRow) // Move Down
            {
                possibleNextPoints.Add(PointInDirection(startPoint, 3));
                if (startCol > endCol)
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 4));
                }
                if (startCol < endCol)
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 2));
                }
            }
            else if (startRow > endRow) // Move Up
            {
                possibleNextPoints.Add(PointInDirection(startPoint, 0));
                if (startCol > endCol)
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 5));
                }
                if (startCol < endCol)
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 1));
                }
            }
            if (startCol < endCol) // Move Right
            {
                // Odd (1,3,5,...) columns can always move up.
                if (startRow > 0 || (startCol%2 == 1 && startCol < size - 1))
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 1));
                }
                // Even (0,2,4,...) columns can always move down.
                if (startRow < size - 1 || (startCol%2 == 0 && startCol < size - 1))
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 2));
                }
            }
            else if (startCol > endCol) // Move Left
            {
                if (startRow > 0 || (startCol%2 == 1))
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 5));
                }
                if (startRow < size - 1 || (startCol%2 == 0 && startCol > 0))
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 4));
                }
            }
            // Move to the next point.
            if (possibleNextPoints.Count <= 0){break;}
            startPoint = possibleNextPoints[Random.Range(0, possibleNextPoints.Count)];
            // Make it passable.
            allTiles[startPoint] = 0;
            if (startPoint == endPoint){break;}
        }
    }

    protected void ConnectRooms(int roomOne, int roomTwo)
    {
        // Go from one room to the other.
        int startPoint = int.Parse(roomDetails[roomOne].Split("|")[0]);
        int endPoint = int.Parse(roomDetails[roomTwo].Split("|")[0]);
        ConnectPoints(startPoint, endPoint);
    }

    protected int GetColumn(int location)
    {
        return location%size;
    }

    protected int PointInDirection(int location, int direction)
    {
        return utility.PointInDirection(location, direction, size);
    }

    protected void TryToMakeRoom()
    {
        int startPoint = Random.Range(0, allTiles.Count);
        // Start: 0 - top left, 1 - top right, 2 - bottom left, 3 - bottom right
        int direction = Random.Range(0, 4);
        // Size is random but rectangular.
        int width = Random.Range(minRoomSize, minRoomSize + (size/minRoomSize)/2);
        int height = Random.Range(minRoomSize, minRoomSize + (size/minRoomSize)/2);
        // Need to check if the size fits and doesn't loop around.
        List<int> roomTiles = new List<int>();
        if (!CheckRoomTiles(roomTiles, startPoint, direction, width, height)){return;}
        // Add the details to the room tiles, can easily recreate the room from the details.
        startPoint = AdjustStart(startPoint, direction);
        string roomDets = startPoint+"|"+direction+"|"+width+"|"+height;
        roomDetails.Add(roomDets);
        // Make the tiles in the room passable.
        int i = 0;
        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                // Close off the borders.
                //if (j == 0 || j == width-1 || k == 0 || k == height-1) // results in unreachable rooms
                if (j == 0 || j == width - 1) // seems ok so far
                //if (k == 0 || k == height-1) // results in unreachable rooms
                {
                    allTiles[roomTiles[i]] = 1;
                }
                // Open up the insides.
                else
                {
                    allTiles[roomTiles[i]] = 0;
                }
                i++;
            }
        }
    }

    protected int AdjustStart(int start, int direction)
    {
        switch (direction)
        {
            case 0:
            return PointInDirection(start, 2);
            case 1:
            return PointInDirection(start, 4);
            case 2:
            return PointInDirection(start, 1);
            case 3:
            return PointInDirection(start, 5);
        }
        return start;
    }
    
    protected bool CheckRoomTiles(List<int> roomTiles, int startPoint, int direction, int width, int height)
    {
        int nextTile = startPoint;
        int startRow = utility.GetRow(startPoint, size);
        int startCol = utility.GetColumn(startPoint, size);
        switch (direction)
        {
            case 0:
                if (startCol + width >= size || startRow + height >= size){return false;}
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (nextTile < 0 || nextTile >= size * size || allTiles[nextTile] == 0){return false;}
                        roomTiles.Add(nextTile);
                        nextTile++;
                    }
                    nextTile -= width;
                    nextTile += size;
                }
                break;
            case 1:
                if (startCol - width < 0 || startRow + height >= size){return false;}
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (nextTile < 0 || nextTile >= size * size || allTiles[nextTile] == 0){return false;}
                        roomTiles.Add(nextTile);
                        nextTile--;
                    }
                    nextTile += width;
                    nextTile += size;
                }
                break;
            case 2:
                if (startCol + width >= size || startRow - height < 0){return false;}
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (nextTile < 0 || nextTile >= size * size || allTiles[nextTile] == 0){return false;}
                        roomTiles.Add(nextTile);
                        nextTile++;
                    }
                    nextTile -= width;
                    nextTile -= size;
                }
                break;
            case 3:
                if (startCol - width < 0 || startRow - height < 0){return false;}
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (nextTile < 0 || nextTile >= size * size || allTiles[nextTile] == 0){return false;}
                        roomTiles.Add(nextTile);
                        nextTile--;
                    }
                    nextTile += width;
                    nextTile -= size;
                }
                break;
        }
        return true;
    }

    protected List<int> GetRoomTiles(string roomDets)
    {
        string[] roomInfo = roomDets.Split("|");
        int startPoint = int.Parse(roomInfo[0]);
        int direction = int.Parse(roomInfo[1]);
        int width = int.Parse(roomInfo[2]);
        int height = int.Parse(roomInfo[3]);
        List<int> roomTiles = new List<int>();
        int nextTile = startPoint;
        switch (direction)
        {
            case 0:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (!(j == 0 || j == width - 1))
                        {
                            roomTiles.Add(nextTile);
                        }
                        nextTile++;
                    }
                    nextTile -= width;
                    nextTile += size;
                }
                break;
            case 1:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (!(j == 0 || j == width - 1))
                        {
                            roomTiles.Add(nextTile);
                        }
                        nextTile--;
                    }
                    nextTile += width;
                    nextTile += size;
                }
                break;
            case 2:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (!(j == 0 || j == width - 1))
                        {
                            roomTiles.Add(nextTile);
                        }
                        nextTile++;
                    }
                    nextTile -= width;
                    nextTile -= size;
                }
                break;
            case 3:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (!(j == 0 || j == width - 1))
                        {
                            roomTiles.Add(nextTile);
                        }
                        nextTile--;
                    }
                    nextTile += width;
                    nextTile -= size;
                }
                break;
        }
        return roomTiles;
    }
}
