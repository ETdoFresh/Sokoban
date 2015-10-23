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
    public GameObject SolidPrefab;
    public GameObject MoveablePrefab;
    public GameObject TargetPrefab;
    public GameObject PlayerPrefab;
    public Level level;
    private int _boxesPushed = 0;

    enum Direction { UP, DOWN, LEFT, RIGHT }

    void OnEnable()
    {
        level = Level.Create(LevelReader.ReadLevel("Assets/Level/level1.txt"));
        grid = GameObject.Find("Ground").GetComponent<MainGrid>();
        grid.SetGrid(level.width, level.height);

        InputController.OnUp += MoveUp;
        InputController.OnDown += MoveDown;
        InputController.OnLeft += MoveLeft;
        InputController.OnRight += MoveRight;

        OnMove += CheckTargets;
    }

    void OnDisable()
    {
        InputController.OnUp -= MoveUp;
        InputController.OnDown -= MoveDown;
        InputController.OnLeft -= MoveLeft;
        InputController.OnRight -= MoveRight;

        OnMove -= CheckTargets;
    }

    void Start()
    {
        GameObject solids = new GameObject() { name = "Solids" };
        GameObject moveables = new GameObject() { name = "Moveable Boxes" };
        GameObject targets = new GameObject() { name = "Targets" };
        for (int x = 0; x < level.width; x++)
            for (int y = 0; y < level.height; y++)
                switch (level.data[x, y])
                {
                    case '1':
                        GameObject solid = Spawn(SolidPrefab, grid.GetCell(x, y));
                        solid.name += " " + solids.transform.childCount;
                        solid.transform.parent = solids.transform;
                        break;
                    case 'S':
                        player = Spawn(PlayerPrefab, grid.GetCell(x, y));
                        playerCell = player.GetComponent<Cell>();
                        break;
                    case 'B':
                        GameObject box = Spawn(MoveablePrefab, grid.GetCell(x, y));
                        box.name += " " + moveables.transform.childCount;
                        box.transform.parent = moveables.transform;
                        break;
                    case 'T':
                        GameObject target = Spawn(TargetPrefab, grid.GetCell(x, y));
                        target.name += " " + targets.transform.childCount;
                        target.transform.parent = targets.transform;
                        grid.GetCell(x, y).GetComponent<CellManager>().gameObjectOnMe = null;
                        break;
                }
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
            GameObject c = grid.grid[cell.x, cell.y];
            CellManager cm = c.GetComponent<CellManager>();
            GameObject onCell = cm.gameObjectOnMe;
            if (onCell == null || onCell.name == "Player")
            {
                allTargetsDone = false;
                break;
            }
        }

        if (targets.GetLength(0) == 0)
            return;

        if (allTargetsDone)
            Debug.Log("Winner winner, chicken dinner!");
    }

    GameObject Spawn(GameObject spawnObject, GameObject cell)
    {
        GameObject newObject = Instantiate(spawnObject);
        newObject.name = spawnObject.name;
        newObject.GetComponent<Cell>().SetCell(cell);
        return newObject;
    }
}
