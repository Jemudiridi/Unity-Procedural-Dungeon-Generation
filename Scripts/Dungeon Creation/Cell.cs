using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum CellType
    {
        Empty,
        Room,
        Hall
    }

    private int roomNumber = -1;

    public Transform spawnPoint;

    private bool isCollapsed = false;
    private List<Room> hallStartRoomList = new List<Room>();
    private CellType collapsedRoomType = CellType.Empty;
    public bool isEnterance;
    public Room boundedRoom;

    public GameObject collapsedobect;

    public List<int> connectedRooms = new List<int>();

    public int gCost;
    public int hCost;
    public int fCost;
    public Cell cameFromCell;


    public  Vector2Int gridPosition;
    private Cell northNeighbour;
    private Cell eastNeighbour;
    private Cell southNeighbour;
    private Cell westNeighbour;

    private void Awake()
    {
        DungeonCreator.OnAnyDungeonMapComplete += DungeonCreator_OnAnyDungeonMapComplete;
    }

    private void DungeonCreator_OnAnyDungeonMapComplete(object sender, System.EventArgs e)
    {
        if (collapsedRoomType == CellType.Room)
        {
            collapsedobect.GetComponent<RoomBehaviour>().ShowValidObjects(this);
        }

        if (collapsedRoomType == CellType.Hall)
        {
            collapsedobect.GetComponent<HallBehaviour>().ShowValidObjects(this);
        }

        if (boundedRoom != null)
        {
            roomNumber = boundedRoom.roomNumber;
        }
    }

    

   

   

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public CellType GetCellType()
    {
        return collapsedRoomType;
    }

    

    public void CollapseCell(CellType roomType)
    {
        this.isCollapsed = true;
        this.collapsedRoomType = roomType;

    }

    public bool IsCollapsed()
    {
        return isCollapsed;
    }

    public void SetCollapsed(bool state)
    {
        this.isCollapsed = state;
    }

   
    public List<Cell> GetAllNeighboursList()
    {
        List<Cell> neighbourList = new List<Cell>();

        if (northNeighbour != null) neighbourList.Add(northNeighbour);
        if (eastNeighbour != null) neighbourList.Add(eastNeighbour);
        if (southNeighbour != null) neighbourList.Add(southNeighbour);
        if (westNeighbour != null) neighbourList.Add(westNeighbour);

        return neighbourList;
    }
    public Cell GetNorthNeighbour()
    {
        return this.northNeighbour;
    }
    public Cell GetSouthNeighbour()
    {
        return this.southNeighbour;
    }
    public Cell GetWestNeighbour()
    {
        return this.westNeighbour;
    }
    public Cell GetEastNeighbour()
    {
        return this.eastNeighbour;
    }

    public void SetNorthNeighbour(Cell northNeighbour)
    {
        this.northNeighbour = northNeighbour;
    }

    public void SetSouthNeighbour(Cell southNeighbour)
    {
        this.southNeighbour = southNeighbour;
    }

    public void SetWestNeighbour(Cell westNeighbour)
    {
        this.westNeighbour = westNeighbour;
    }

    public void SetEastNeighbour(Cell eastNeighbour)
    {
        this.eastNeighbour = eastNeighbour;
    }

    public void AddHallStartRoomList(Room roomToAdd)
    {
        hallStartRoomList.Add(roomToAdd);
    }

    public bool IsHallStartRoomContainThisRoom(Room room)
    {
        foreach(Room roomToSearch in hallStartRoomList)
        {

            if (roomToSearch == room) return true;
        }
        return false;
    }

    public List<Room> GetHallStartRoomList()
    {
        return hallStartRoomList;
    }

    private void OnDestroy()
    {
        DungeonCreator.OnAnyDungeonMapComplete -= DungeonCreator_OnAnyDungeonMapComplete;
    }
}
