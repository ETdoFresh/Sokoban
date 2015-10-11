using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;
    public GameObject cell;
    public bool isSolid;

    public void SetCell(GameObject cell)
    {
        if (this.cell != null)
            this.cell.GetComponent<CellManager>().gameObjectOnMe = null;

        this.cell = cell;
        cell.GetComponent<CellManager>().gameObjectOnMe = gameObject;
        x = cell.GetComponent<CellManager>()._x;
        y = cell.GetComponent<CellManager>()._y;
        transform.position = cell.transform.position;
    }
}
