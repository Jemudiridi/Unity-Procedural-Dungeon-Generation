using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HallBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject wallNorth;
    [SerializeField] private GameObject wallEast;
    [SerializeField] private GameObject wallSouth;
    [SerializeField] private GameObject WallWest;
    [SerializeField] private GameObject arch;
    [SerializeField] private GameObject braizer;

    [Header("Enterences")]
    [SerializeField] private GameObject enterenceNorth;
    [SerializeField] private GameObject enterenceEast;
    [SerializeField] private GameObject enterenceSouth;
    [SerializeField] private GameObject enterenceWest;

    private bool wallNorthActive = true;
    private bool wallEastActive = true;
    private bool wallSouthActive = true;
    private bool WallWestActive = true;

    private int braizerRandom = 0;

    private void Start()
    {
        wallNorth.gameObject.SetActive(true);
        wallEast.gameObject.SetActive(true);
        wallSouth.gameObject.SetActive(true);
        WallWest.gameObject.SetActive(true);
    }




    public void ShowValidObjects(Cell cell)
    {
        if (cell.GetNorthNeighbour() != null)
        {
            if (cell.GetNorthNeighbour().GetCellType() == Cell.CellType.Hall)
            {
                //wallNorth.gameObject.SetActive(false);
                wallNorthActive =false;
                Destroy(wallNorth.gameObject);
            }

            if (cell.GetHallStartRoomList().Count > 0)
            {
                if (cell.IsHallStartRoomContainThisRoom(cell.GetNorthNeighbour().boundedRoom))
                {
                    Destroy(wallNorth.gameObject);
                    enterenceNorth.SetActive(true);
                }
            }
        }
        if (cell.GetEastNeighbour() != null)
        {
            if (cell.GetEastNeighbour().GetCellType() == Cell.CellType.Hall)
            {
                // wallEast.gameObject.SetActive(false);
                wallEastActive = false;

                Destroy(wallEast.gameObject);

            }

            if (cell.GetHallStartRoomList().Count > 0)
            {
                if (cell.IsHallStartRoomContainThisRoom(cell.GetEastNeighbour().boundedRoom))
                {
                    Destroy(wallEast.gameObject);
                    enterenceEast.SetActive(true);
                }
            }
        }

        if (cell.GetSouthNeighbour() != null)
        {
            if (cell.GetSouthNeighbour().GetCellType() == Cell.CellType.Hall)
            {
                // wallSouth.gameObject.SetActive(false);
                wallSouthActive = false;

                Destroy(wallSouth.gameObject);
            }

            if(cell.GetHallStartRoomList().Count > 0)
            {
                if(cell.IsHallStartRoomContainThisRoom(cell.GetSouthNeighbour().boundedRoom))
                {
                    Destroy(wallSouth.gameObject);
                    enterenceSouth.SetActive(true);
                }
            }
        }

        if (cell.GetWestNeighbour() != null)
        {
            if (cell.GetWestNeighbour().GetCellType() == Cell.CellType.Hall)
            {
                // WallWest.gameObject.SetActive(false);
                WallWestActive = false;

                Destroy(WallWest.gameObject);
            }

            if (cell.GetHallStartRoomList().Count > 0)
            {
                if (cell.IsHallStartRoomContainThisRoom(cell.GetWestNeighbour().boundedRoom))
                {
                    Destroy(WallWest.gameObject);
                    enterenceWest.SetActive(true);
                }
            }

        }
        SetArch();


    }

    private void SetDoors()
    {

    }
    private void SetArch()
    {
        if(!WallWestActive )
        { 
            if(!wallSouthActive || !wallNorthActive)
            {
                arch.SetActive(true);
            }
        }

        if (!wallEastActive)
        {
            if (!wallSouthActive || !wallNorthActive)
            {
                arch.SetActive(true);
            }
        }

        if (!wallSouthActive)
        {
            if (!WallWestActive || !wallEastActive)
            {
                arch.SetActive(true);
            }
        }

        if (!wallNorthActive)
        {
            if (!WallWestActive || !wallEastActive)
            {
                arch.SetActive(true);
            }
        }
        if(braizerRandom == 0)
        {
            braizerRandom = Random.Range(1, 4);
        }else
        {

        }

        if (braizerRandom > 1 && arch.activeSelf == true)
        {
            braizer.SetActive(false);
        }
        
    }
}
