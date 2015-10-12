using UnityEngine;
using System.Collections;
using System;

public class MainGrid : MonoBehaviour
{
    public delegate void StartHandler(MainGrid grid);
    static public event StartHandler OnStart = delegate {};

    public int width = 10;
    public int height = 10;
    public GameObject[,] grid;
    public float gridWidth;
    public float gridHeight;
    public float cellWidth;
    public float cellHeight;

    void OnEnable()
    {
        if (width <= 0) width = 10;
        if (height <= 0) height = 10;

        grid = new GameObject[width, height];
        gridWidth = transform.localScale.x;
        gridHeight = transform.localScale.y;
        cellWidth = gridWidth / width;
        cellHeight = gridHeight / height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cell = new GameObject();
                cell.transform.parent = transform;
                cell.name = "Cell[" + x + "," + y + "]";
                float posX = (-gridWidth / 2) + (x * cellWidth) + (cellWidth / 2);
                float posY = (-gridHeight/ 2) + (y * cellHeight) + (cellHeight/ 2);
                cell.transform.position = new Vector3(posX, 0, posY);
                cell.transform.rotation = Quaternion.identity;
                CellManager cellManager = cell.AddComponent<CellManager>();
                cellManager.SetCell(x, y);
                grid[x, y] = cell;
            }
        }
    }
    void Start()
    {
        OnStart(this);
    }

    public GameObject GetCell(int x, int y)
    {
        if (0 > x || x >= gridWidth)
            return null;

        if (0 > y || y >= gridHeight)
            return null;

        return grid[x, y];
    }

    public void AssignClosestCell(GameObject gameObject)
    {
        // EJlol3: Suggestion - Calculate cell instead of finding closest cell
        Vector3 position = gameObject.transform.position;
        Cell gObjCell = gameObject.GetComponent<Cell>();
        GameObject closestCell = grid[gObjCell.x, gObjCell.y];
        Vector3 cellPosition = closestCell.transform.position;
        float minDistance = Vector3.Distance(position, cellPosition);
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 testPosition = grid[x, y].transform.position;
                float distance = Vector3.Distance(position, testPosition);
                if (distance < minDistance)
                {
                    closestCell = grid[x, y];
                    minDistance = distance;
                }
            }
        gObjCell.SetCell(closestCell);
    }
}
