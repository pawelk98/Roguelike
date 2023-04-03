using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public class Cell
    {
        public enum Direction { Up, Down, Left, Right }
        public bool visited; 
        public bool[] status = new bool[4]; //up, down, left, right
    }

    public static bool generated;

    public Vector2Int size;
    public int mazeStart = 0;
    public int mazeEnd;
    public int mazeMaxLength;
    public int offTrackDoorChance;
    public int treasureRoomChance;
    public Vector2 offset;

    public GameObject roomPrefab;
    public PlayerSpawn playerSpawn;
    public NavMeshSurface navMeshSurface;
    public RoomGenerator roomGenerator;

    List<Cell> board;


    void Start()
    {
        MazeGeneration();
    }

    void GenerateDungeon()
    {
        GameObject startingRoom = null;
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int cellID = x + y * size.x;
                if (board[cellID].visited)
                {
                    if(cellID != mazeEnd) //boss room has only one entrance
                    {
                        List<int> neighbors = CheckNeighbors(x + y * size.x, true);
                        for (int i = 0; i < neighbors.Count; i++)
                        {
                            if (Random.Range(0, 99) < offTrackDoorChance && neighbors[i] != mazeEnd)
                                AddPath(cellID, neighbors[i]);
                        }
                    }

                    if (cellID == mazeStart)
                    {
                        startingRoom = roomGenerator.GenerateRoom(RoomGenerator.RoomType.Start, new Vector3(x * offset.x, 0, y * offset.y), board[cellID].status);
                    }
                    else if(cellID == mazeEnd)
                    {
                        roomGenerator.GenerateRoom(RoomGenerator.RoomType.Boss, new Vector3(x * offset.x, 0, y * offset.y), board[cellID].status);
                    }
                    else if(Random.Range(0,99) < treasureRoomChance)
                    {
                        roomGenerator.GenerateRoom(RoomGenerator.RoomType.Treasure, new Vector3(x * offset.x, 0, y * offset.y), board[cellID].status);
                    }
                    else
                    {
                        roomGenerator.GenerateRoom(RoomGenerator.RoomType.Default, new Vector3(x * offset.x, 0, y * offset.y), board[cellID].status);
                    }
                }
            }
        }

        navMeshSurface.BuildNavMesh();
        generated = true;
        playerSpawn.SpawnPlayer(startingRoom);
    }

    void MazeGeneration()
    {
        board = new List<Cell>();

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = mazeStart;

        Stack<int> path = new Stack<int>();

        while (true)
        {
            board[currentCell].visited = true;

            if (currentCell == mazeEnd || path.Count == mazeMaxLength)
            {
                mazeEnd = currentCell;
                break;
            }

            List<int> neighbors = CheckNeighbors(currentCell, false);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)];
                AddPath(currentCell, newCell);
                currentCell = newCell;
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell, bool visited)
    {
        List<int> neighbors = new List<int>();

        //up
        if (cell + size.x < board.Count && board[cell + size.x].visited == visited)
        {
            neighbors.Add(cell + size.x);
        }
        //down
        if (cell - size.x >= 0 && board[cell - size.x].visited == visited)
        {
            neighbors.Add(cell - size.x);
        }
        //left
        if (cell % size.x != 0 && board[cell - 1].visited == visited)
        {
            neighbors.Add(cell - 1);
        }
        //right
        if ((cell + 1) % size.x != 0 && board[cell + 1].visited == visited)
        {
            neighbors.Add(cell + 1);
        }

        return neighbors;
    }

    Cell.Direction NextCellDirection(int source, int destination)
    {
        if (destination > source)  //up or right
        {
            if (destination - 1 == source)
            {
                return Cell.Direction.Right;
            }
            else
            {
                return Cell.Direction.Up;
            }
        }
        else   //down or left
        {
            if (destination + 1 == source)
            {
                return Cell.Direction.Left;
            }
            else
            {
                return Cell.Direction.Down;
            }
        }
    }

    void AddPath(int cell1, int cell2)
    {
        if (NextCellDirection(cell1, cell2) == Cell.Direction.Right)
        {
            board[cell1].status[3] = true;
            board[cell2].status[2] = true;
        }
        else if (NextCellDirection(cell1, cell2) == Cell.Direction.Up)
        {
            board[cell1].status[0] = true;
            board[cell2].status[1] = true;
        }
        else if (NextCellDirection(cell1, cell2) == Cell.Direction.Left)
        {
            board[cell1].status[2] = true;
            board[cell2].status[3] = true;
        }
        else if (NextCellDirection(cell1, cell2) == Cell.Direction.Down)
        {
            board[cell1].status[1] = true;
            board[cell2].status[0] = true;
        }
    }
}
