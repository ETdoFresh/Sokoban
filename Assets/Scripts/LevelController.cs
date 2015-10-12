using UnityEngine;
using System.Collections;
using System;

public class LevelController : MonoBehaviour
{
    static public event Action OnMove = delegate { };

    public MainGrid grid;
    public GameObject player;
    public Cell playerCell;
    public int boxStrength = 1;
    private int _boxesPushed = 0;

    enum Direction { UP, DOWN, LEFT, RIGHT }

    void OnEnable()
    {
        MainGrid.OnStart += OnGridStart;
        Player.OnStart += onPlayerStart;

        InputController.OnUp += MoveUp;
        InputController.OnDown += MoveDown;
        InputController.OnLeft += MoveLeft;
        InputController.OnRight += MoveRight;

        OnMove += CheckTargets;
    }

    void OnDisable()
    {
        MainGrid.OnStart -= OnGridStart;
        Player.OnStart -= onPlayerStart;

        InputController.OnUp -= MoveUp;
        InputController.OnDown -= MoveDown;
        InputController.OnLeft -= MoveLeft;
        InputController.OnRight -= MoveRight;

        OnMove -= CheckTargets;
    }

    void OnGridStart(MainGrid grid)
    {
        this.grid = grid;
        StartLevel();
    }

    void onPlayerStart(GameObject player)
    {
        this.player = player;
        playerCell = player.GetComponent<Cell>();
        StartLevel();
    }

    void StartLevel()
    {
        if (player == null) return;
        if (grid == null) return;

        GameObject[] mapObjects;
        mapObjects = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject mapObject in mapObjects)
        {
            grid.AssignClosestCell(mapObject);
            mapObject.GetComponent<Cell>().cell.GetComponent<CellManager>().gameObjectOnMe = null;
        }

        mapObjects = GameObject.FindGameObjectsWithTag("Map Object");
        foreach (GameObject mapObject in mapObjects)
            grid.AssignClosestCell(mapObject);
    }

    void MoveUp() { Move(playerCell, Direction.UP); }
    void MoveDown() { Move(playerCell, Direction.DOWN); }
    void MoveLeft() { Move(playerCell, Direction.LEFT); }
    void MoveRight() { Move(playerCell, Direction.RIGHT); }

    void Move(Cell cell, Direction direction)
    {
        GameObject destinationCell = null;
        int x = cell.x;
        int y = cell.y;
        switch (direction)
        {
            case Direction.UP:
                destinationCell = grid.GetCell(x, y + 1);
                break;
            case Direction.DOWN:
                destinationCell = grid.GetCell(x, y - 1);
                break;
            case Direction.LEFT:
                destinationCell = grid.GetCell(x - 1, y);
                break;
            case Direction.RIGHT:
                destinationCell = grid.GetCell(x + 1, y);
                break;
        }

        if (isValidMove(destinationCell, direction))
            cell.SetCell(destinationCell);

        _boxesPushed = 0;
        OnMove();
    }

    bool isValidMove(GameObject cell, Direction direction)
    {
        if (cell == null)
            return false;

        if (!isPassable(cell))
            return false;

        if (!isMovable(cell, direction))
            return false;

        return true;
    }

    private bool isMovable(GameObject cell, Direction direction)
    {
        CellManager cellManager = cell.GetComponent<CellManager>();
        if (cellManager.gameObjectOnMe != null)
        {
            Cell cellsGameObjectCell = cellManager.gameObjectOnMe.GetComponent<Cell>();
            _boxesPushed++;
            if (_boxesPushed <= boxStrength)
                Move(cellsGameObjectCell, direction);

            if (cellManager.gameObjectOnMe == null)
                return true;

            return false;
        }
        return true;
    }

    private bool isPassable(GameObject cell)
    {
        CellManager cellManager = cell.GetComponent<CellManager>();
        if (cellManager.gameObjectOnMe != null)
        {
            Cell cellsGameObjectCell = cellManager.gameObjectOnMe.GetComponent<Cell>();
            if (cellsGameObjectCell != null)
                return !cellsGameObjectCell.isSolid;
        }
        return true;
    }

    private void CheckTargets()
    {
        bool allTargetsDone = true;
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        foreach(GameObject target in targets)
        {
            Cell cell = target.GetComponent<Cell>();
            GameObject onCell = grid.grid[cell.x, cell.y].GetComponent<CellManager>().gameObjectOnMe;
            if (onCell == null || onCell.name == "Player")
            {
                allTargetsDone = false;
                break;
            }
        }

        if (allTargetsDone)
            Debug.Log("Winner winner, chicken dinner!");
    }
}
