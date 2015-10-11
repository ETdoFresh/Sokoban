using UnityEngine;
using System.Collections;
using System;

public class LevelController : MonoBehaviour
{
    public MainGrid grid;
    public GameObject player;

    enum Direction { UP, DOWN, LEFT, RIGHT }

    void OnEnable()
    {
        MainGrid.OnStart += OnGridStart;
        Player.OnStart += onPlayerStart;

        InputController.OnUp += MoveUp;
        InputController.OnDown += MoveDown;
        InputController.OnLeft += MoveLeft;
        InputController.OnRight += MoveRight;
    }

    void OnGridStart(MainGrid grid)
    {
        this.grid = grid;
        StartLevel();
    }

    void onPlayerStart(GameObject player)
    {
        this.player = player;
        StartLevel();
    }

    void StartLevel()
    {
        if (player == null) return;
        if (grid == null) return;

        GameObject startingCell = grid.grid[0, 0];
        player.GetComponent<Cell>().SetCell(startingCell);

        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map Object");
        foreach (GameObject mapObject in mapObjects)
            grid.AssignClosestCell(mapObject);
    }

    void MoveUp() { Move(Direction.UP); }
    void MoveDown() { Move(Direction.DOWN); }
    void MoveLeft() { Move(Direction.LEFT); }
    void MoveRight() { Move(Direction.RIGHT); }

    void Move(Direction direction)
    {
        GameObject destinationCell = null;
        int x = player.GetComponent<Cell>().x;
        int y = player.GetComponent<Cell>().y;
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
        if (isValidMove(destinationCell))
            player.GetComponent<Cell>().SetCell(destinationCell);
    }

    bool isValidMove(GameObject cell)
    {
        if (cell == null)
            return false;

        if (!isEmpty(cell))
            return false;

        return true;
    }

    private bool isEmpty(GameObject cell)
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
}
