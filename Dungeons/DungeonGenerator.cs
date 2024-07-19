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
    public List<int> allTiles = new List<int>();
    public List<int> rooms = new List<int>();
    public List<string> roomDetails = new List<string>();
    public List<int> unconnectedRooms = new List<int>();
    //public List<int> impassableTiles = new List<int>();
    public List<string> GenerateDungeon(int newSize = 36)
    {
        size = newSize;
        if (size < minSize){size = minSize;}
        Reset();
        List<string> dungeonData = new List<string>();
        // Get base size.
        GetTiles();
        // Make a maze.
            // Sprinkle in an amount of randomly sized, nonoverlapping rooms.
            // Make passages connected the rooms.
        MakeRooms();
        // Get a starting point.
            // Inside a randomly selected room.
        int start = GetRandomPointInRoom(Random.Range(0, rooms.Count));
        // Get an exit.
            // Inside a randomly selected room.
        int end = GetRandomPointInRoom(Random.Range(0, rooms.Count));
        // Get treasure locations.
            // Inside 1+ randomly selected room(s).
        // Get enemies.
            // Starting amount equal to size or sqrt(size)?
            // Enemies moves every turn and spawn every X turns.
        dungeonData.Add(utility.ConvertIntListToString(allTiles));
        dungeonData.Add(start.ToString());
        dungeonData.Add(end.ToString());
        return dungeonData;
    }

    protected int GetRandomPointInRoom(int roomNum)
    {
        List<int> roomTiles = GetRoomTiles(roomDetails[roomNum]);
        int start = roomTiles[Random.Range(0, roomTiles.Count)];
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
        rooms.Clear();
        roomDetails.Clear();
        unconnectedRooms.Clear();
        // Try X times or until you get the min amount of rooms.
        for (int i = 0; i < tries; i++)
        {
            TryToMakeRoom();
        }
        // If you fail to make any rooms then the whole floor is one big room.
        if (rooms.Count <= 1)
        {
            for (int i = 0; i < size*size; i++)
            {
                allTiles[i] = 0;
            }
        }
        // Otherwise try to connect all the rooms.
        else
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                int roomOne = rooms[Random.Range(0, rooms.Count)];
                int roomTwo = rooms[Random.Range(0, rooms.Count)];
                while (roomOne == roomTwo)
                {
                    roomTwo = rooms[Random.Range(0, rooms.Count)];
                }
                ConnectRooms(roomOne, roomTwo);
            }
        }
    }

    protected void ConnectRooms(int roomOne, int roomTwo)
    {
        // Go from one room to the other.
        int startPoint = int.Parse(roomDetails[roomOne].Split("|")[0]);
        int endPoint = int.Parse(roomDetails[roomTwo].Split("|")[0]);
        int startRow = (startPoint/size);
        int startCol = GetColumn(startPoint);
        int endRow = (endPoint/size);
        int endCol = GetColumn(endPoint);
        List<int> possibleNextPoints = new List<int>();
        for (int i = 0; i < size*size; i++)
        {
            if (startPoint == endPoint){break;}
            possibleNextPoints.Clear();
            startRow = (startPoint/size);
            startCol = GetColumn(startPoint);
            // Randomly path in the general direction of the end point.
            if (startRow < endRow) // Move Down
            {
                possibleNextPoints.Add(PointInDirection(startPoint, 3));
                if (startCol > 0)
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 4));
                }
                if (startCol < size - 1)
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 2));
                }
            }
            else if (startRow > endRow) // Move Up
            {
                possibleNextPoints.Add(PointInDirection(startPoint, 0));
                if (startCol > 0)
                {
                    possibleNextPoints.Add(PointInDirection(startPoint, 5));
                }
                if (startCol < size - 1)
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
        }
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
        int width = Random.Range(minRoomSize, minRoomSize + size/minRoomSize);
        int height = Random.Range(minRoomSize, minRoomSize + size/minRoomSize);
        List<int> roomTiles = new List<int>();
        if (CheckRoomTiles(roomTiles, startPoint, direction, width, height))
        {
            // Add the room to the list of rooms.
            unconnectedRooms.Add(rooms.Count);
            rooms.Add(rooms.Count);
            // Add the details to the room tiles, can easily recreate the room from the details.
            string roomDets = startPoint+"|"+direction+"|"+width+"|"+height;
            roomDetails.Add(roomDets);
            // Make the tiles in the room passable.
            int i = 0;
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    if (j == 0 || j == width-1 || k == 0 || k == height-1)
                    {
                        allTiles[roomTiles[i]] = 1;
                        //impassableTiles.Add(roomTiles[i]);
                    }
                    else
                    {
                        allTiles[roomTiles[i]] = 0;
                        //impassableTiles.Remove(roomTiles[i]);
                    }
                    i++;
                }
            }
        }
    }

    protected bool CheckRoomTiles(List<int> roomTiles, int startPoint, int direction, int width, int height)
    {
        int nextTile = startPoint;
        switch (direction)
        {
            case 0:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        // Can't go out of bounds or overlap with other rooms.
                        if (nextTile < 0 || nextTile >= size * size || allTiles[nextTile] == 0){return false;}
                        roomTiles.Add(nextTile);
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
                        if (nextTile < 0 || nextTile >= size * size || allTiles[nextTile] == 0){return false;}
                        roomTiles.Add(nextTile);
                        nextTile--;
                    }
                    nextTile += width;
                    nextTile -= size;
                }
                break;
            case 2:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (nextTile < 0 || nextTile >= size * size || allTiles[nextTile] == 0){return false;}
                        roomTiles.Add(nextTile);
                        nextTile++;
                    }
                    nextTile += width;
                    nextTile -= size;
                }
                break;
            case 3:
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
                        roomTiles.Add(nextTile);
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
                        roomTiles.Add(nextTile);
                        nextTile--;
                    }
                    nextTile += width;
                    nextTile -= size;
                }
                break;
            case 2:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        roomTiles.Add(nextTile);
                        nextTile++;
                    }
                    nextTile += width;
                    nextTile -= size;
                }
                break;
            case 3:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        roomTiles.Add(nextTile);
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
