using UnityEngine;
using System.Collections;
using System;

public class LevelController : MonoBehaviour
{
    public MainGrid grid;
    public Player player;

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

    void onPlayerStart(Player player)
    {
        this.player = player;
        StartLevel();
    }

    void StartLevel()
    {
        if (player == null) return;
        if (grid == null) return;

        var startingCell = grid.grid[0, 0];
        player.SetCell(startingCell);
    }

    void MoveUp() { Move(Direction.UP); }
    void MoveDown() { Move(Direction.DOWN); }
    void MoveLeft() { Move(Direction.LEFT); }
    void MoveRight() { Move(Direction.RIGHT); }

    void Move(Direction direction)
    {
        switch(direction)
        {
            case Direction.UP:
                Debug.Log("Up");
                if (player.cell.y + 1 < grid.gridHeight)
                    player.SetCell(grid.grid[player.cell.x, player.cell.y + 1]);
                break;
            case Direction.DOWN:
                Debug.Log("Down");
                if (player.cell.y - 1 >= 0)
                    player.SetCell(grid.grid[player.cell.x, player.cell.y - 1]);
                break;
            case Direction.LEFT:
                Debug.Log("Left");
                if (player.cell.x - 1 >= 0)
                    player.SetCell(grid.grid[player.cell.x - 1, player.cell.y]);
                break;
            case Direction.RIGHT:
                Debug.Log("Right");
                if (player.cell.x + 1 < grid.gridWidth)
                    player.SetCell(grid.grid[player.cell.x + 1, player.cell.y]);
                break;
        }
    }
}
