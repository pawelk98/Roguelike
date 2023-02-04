using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited;
        public bool[] status = new bool[4]; //up, down, left, right
    }

    public Vector2Int size;
    public int mazeStart = 0;
    public int mazeDestination;
    public int mazeMaxLength;
    public float offTrackDoorChance;
    public Vector2 offset;
    public GameObject roomPrefab;

    List<Cell> board;


    void Start()
    {
        MazeGenerator();
    }


    void Update()
    {
        
    }

    void GenerateDungeon()
    {
        for(int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Cell currentCell = board[x + y * size.x];
                if (currentCell.visited)
                {
                    GameObject room = Instantiate(roomPrefab, new Vector3(x * offset.x, 0, y * offset.y), Quaternion.identity, transform);
                    room.GetComponent<RoomGenerator>().SetConnections(currentCell.status);
                    room.name = "Room x:" + x + " y:" + y;
                }
            }
        }
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int y = 0; y < size.y; y++)
        {
            for(int x = 0; x < size.x; x++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = mazeStart;

        Stack<int> path = new Stack <int>();

        while (true)
        {
            board[currentCell].visited = true;

            if (currentCell == mazeDestination || path.Count == mazeMaxLength)
            {
                break;
            }

            List<int> neighbors = checkNeighbors(currentCell);

            if(neighbors.Count == 0)
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

                if (newCell > currentCell)  //up or right
                {
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
                else   //down or left
                {
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    List<int> checkNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //up
        if (cell + size.x < board.Count && !board[cell + size.x].visited)
        {
            neighbors.Add(cell + size.x);
        }
        //down
        if (cell - size.x >= 0 && !board[cell - size.x].visited)
        {
            neighbors.Add(cell - size.x);
        }
        //left
        if (cell % size.x != 0 && !board[cell - 1].visited)
        {
            neighbors.Add(cell - 1);
        }
        //right
        if ((cell + 1) % size.x != 0 && !board[cell + 1].visited)
        {
            neighbors.Add(cell + 1);
        }

        return neighbors;
    }
}
