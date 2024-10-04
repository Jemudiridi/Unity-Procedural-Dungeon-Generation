using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject wallNorth;
    [SerializeField] private GameObject wallEast;
    [SerializeField] private GameObject wallSouth;
    [SerializeField] private GameObject WallWest;

    [SerializeField] private GameObject enteranceNorth;
    [SerializeField] private GameObject enteranceEast;
    [SerializeField] private GameObject enteranceSouth;
    [SerializeField] private GameObject enteranceWest;

    [SerializeField] private List<GameObject> TorchesList;

    private void Start()
    {
        wallNorth.gameObject.SetActive(true);
        wallEast.gameObject.SetActive(true);
        wallSouth.gameObject.SetActive(true);
        WallWest.gameObject.SetActive(true);
        enteranceNorth.gameObject.SetActive(false);
        enteranceEast.gameObject.SetActive(false);
        enteranceSouth.gameObject.SetActive(false);
        enteranceWest.gameObject.SetActive(false);

        ShowRandomTorches();
    }




    public void ShowValidObjects(Cell cell)
    {
        if (cell.GetNorthNeighbour() != null)
        {
            if (cell.GetNorthNeighbour().GetCellType() == Cell.CellType.Room)
            {
                wallNorth.gameObject.SetActive(false);

            }

            if (cell.GetNorthNeighbour().IsHallStartRoomContainThisRoom(cell.boundedRoom) && cell.GetNorthNeighbour().isEnterance)
            {
                wallNorth.gameObject.SetActive(false);
                enteranceNorth.gameObject.SetActive(true);
            }
        }
        if (cell.GetEastNeighbour() != null)
        {
            if (cell.GetEastNeighbour().GetCellType() == Cell.CellType.Room)
            {
                wallEast.gameObject.SetActive(false);
            }
            if (cell.GetEastNeighbour().IsHallStartRoomContainThisRoom(cell.boundedRoom) && cell.GetEastNeighbour().isEnterance)
            {
                wallEast.gameObject.SetActive(false);
                enteranceEast.gameObject.SetActive(true);
            }
        }

        if (cell.GetSouthNeighbour() != null)
        {
            if (cell.GetSouthNeighbour().GetCellType() == Cell.CellType.Room)
            {
                wallSouth.gameObject.SetActive(false);
            }
            if (cell.GetSouthNeighbour().IsHallStartRoomContainThisRoom(cell.boundedRoom) && cell.GetSouthNeighbour().isEnterance)
            {
                wallSouth.gameObject.SetActive(false);
                enteranceSouth.gameObject.SetActive(true);
            }
        }

        if (cell.GetWestNeighbour() != null)
        {
            if (cell.GetWestNeighbour().GetCellType() == Cell.CellType.Room)
            {
                WallWest.gameObject.SetActive(false);
            }
            if (cell.GetWestNeighbour().IsHallStartRoomContainThisRoom(cell.boundedRoom) && cell.GetWestNeighbour().isEnterance)
            {
                WallWest.gameObject.SetActive(false);
                enteranceWest.gameObject.SetActive(true);
            }
        }

    }

    private void ShowRandomTorches()
    {
        foreach(GameObject torch in TorchesList)
        {
            int i = Random.Range(0, 3);
            if (i == 0) 
            { 
                torch.gameObject.SetActive(false);
            }
        }
    }
}
