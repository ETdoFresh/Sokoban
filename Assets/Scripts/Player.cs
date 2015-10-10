using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public delegate void StartHandler(Player player);
    static public event StartHandler OnStart = delegate { };

    public Cell cell;

    void Start()
    {
        OnStart(this);
    }

    public void SetCell(Cell cell)
    {
        this.cell = cell;
        transform.position = new Vector3(cell.positionX, 0, cell.positionY);
    }
}
