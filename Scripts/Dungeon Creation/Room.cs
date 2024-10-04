using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class Room
{

    public RectInt bounds;
    public RectInt buffer;

    public Rect outerSpawnAreaOut;
    public Rect outerSpawnAreaIn;
    public Rect innerSpawnArea;


    [Header("Corners")]
    public Rect cornerNE;
    public Rect cornerNW;
    public Rect cornerSE;
    public Rect cornerSW;

    [Header("Edge")]
    public Rect edgeN;
    public Rect edgeE;
    public Rect edgeS;
    public Rect edgeW;


    public RoomSO selectedRoomSO;

    public List<Transform> spawnPointList;

    //public List<DungeonObjectSO> instantiatedDOSOList;

    public int roomNumber;

    public List<Cell> roomCells;

    public List<Cell> possibleHallStartCells;

    public bool isPossibleHallStartCellsSetted = false;

    public Cell selectedhallStartCell;

    public int roomEnterenceCount;

    public List<int> connectedRooms = new List<int>();
    public Room(Vector2Int location, Vector2Int size, int cellVolume)
    {
       // instantiatedDOSOList = new List<DungeonObjectSO>();

        spawnPointList = new List<Transform>();

        roomCells = new List<Cell>();

        possibleHallStartCells = new List<Cell>();

        bounds = new RectInt(location, size);


        buffer = new RectInt(location - new Vector2Int(cellVolume, cellVolume), size + new Vector2Int(2 * cellVolume,2 * cellVolume));


        


        outerSpawnAreaIn = new Rect(location + new Vector2(cellVolume / 7f * 2.5f, cellVolume / 7f * 2.5f), size - new Vector2(cellVolume / 7f * 2.5f * 2, cellVolume / 7f * 2.5f *2 ));

        outerSpawnAreaOut = new Rect(location + new Vector2(cellVolume / 7f * 2f, cellVolume / 7f * 2f), size - new Vector2(cellVolume / 7f * 2f * 2 , cellVolume / 7f * 2f * 2));


        innerSpawnArea = new Rect(location + new Vector2(cellVolume / 2f, cellVolume / 2f), size - new Vector2(cellVolume  , cellVolume ));

        SetCornersForOuterSpawn();
        SetEdgesForOuterSpawn();



    }

    private void SetEdgesForOuterSpawn()
    {
        edgeN.xMin = innerSpawnArea.xMin;
        edgeN.yMin = cornerNW.yMin;
        edgeN.xMax = innerSpawnArea.xMax;
        edgeN.yMax = cornerNE.yMax;

        edgeE.xMin = cornerSE.xMin;
        edgeE.yMin = innerSpawnArea.yMin;
        edgeE.xMax = cornerNE.xMax;
        edgeE.yMax = innerSpawnArea.yMax;

        edgeS.xMin = innerSpawnArea.xMin;
        edgeS.yMin = cornerSW.yMin;
        edgeS.xMax = innerSpawnArea.xMax;
        edgeS.yMax = cornerSE.yMax;

        edgeW.xMin = cornerSW.xMin;
        edgeW.yMin = innerSpawnArea.yMin;
        edgeW.xMax = cornerNW.xMax;
        edgeW.yMax = innerSpawnArea.yMax;
    }

    private void SetCornersForOuterSpawn()
    {
        cornerNW.xMin = outerSpawnAreaOut.xMin;
        cornerNW.yMin = outerSpawnAreaIn.yMax;
        cornerNW.xMax = outerSpawnAreaIn.xMin;
        cornerNW.yMax = outerSpawnAreaOut.yMax;

        cornerNE.xMin = outerSpawnAreaIn.xMax;
        cornerNE.yMin = outerSpawnAreaIn.yMax;
        cornerNE.xMax = outerSpawnAreaOut.xMax;
        cornerNE.yMax = outerSpawnAreaOut.yMax;

        cornerSW.xMin = outerSpawnAreaOut.xMin;
        cornerSW.yMin = outerSpawnAreaOut.yMin;
        cornerSW.xMax = outerSpawnAreaIn.xMin;
        cornerSW.yMax = outerSpawnAreaIn.yMin;

        cornerSE.xMin = outerSpawnAreaIn.xMax;
        cornerSE.yMin = outerSpawnAreaOut.yMin;
        cornerSE.xMax = outerSpawnAreaOut.xMax;
        cornerSE.yMax = outerSpawnAreaIn.yMin;
    }

    public List<Rect> GetAllCornersList()
    {
        List<Rect> allCorners = new List<Rect>();
        allCorners.Add(cornerNW);
        allCorners.Add(cornerNE);
        allCorners.Add(cornerSW);
        allCorners.Add(cornerSE);
        return allCorners;
    }

    public List<Rect> GetAllEdgesList()
    {
        List<Rect> allEdges = new List<Rect>();
        allEdges.Add(edgeN);
        allEdges.Add(edgeE);
        allEdges.Add(edgeS);
        allEdges.Add(edgeW);
        return allEdges;
    }
    public void SetPossibleHallStartCells()
    {
        if (isPossibleHallStartCellsSetted) return;
        foreach (Cell roomCell in this.roomCells)
        {
            foreach (Cell neighbourCell in roomCell.GetAllNeighboursList())
            {
                if (!neighbourCell.IsCollapsed())
                {
                    if(!this.possibleHallStartCells.Contains(neighbourCell)) this.possibleHallStartCells.Add(neighbourCell);
                    isPossibleHallStartCellsSetted = true;
                }
            }

        }

    }


    public Cell SelectHallStartCell()
    {
        List<Cell> newPossibleCellList = new List<Cell>();
        newPossibleCellList.AddRange(this.possibleHallStartCells);
        int randomIndex = Random.Range(0, possibleHallStartCells.Count);

        Cell cell = possibleHallStartCells[randomIndex];


        foreach(Cell cellOther in possibleHallStartCells)
        {
            if (cellOther == cell) continue;
            if ( cellOther.gridPosition.x == cell.gridPosition.x || cellOther.gridPosition.y == cell.gridPosition.y)
            {
                newPossibleCellList.Remove(cellOther);
            }
        }
        this.possibleHallStartCells = newPossibleCellList;

        

        if (selectedhallStartCell == null)
        {
            selectedhallStartCell = cell;
            selectedhallStartCell.AddHallStartRoomList(this);
            roomEnterenceCount++;
            return selectedhallStartCell;
        } else
        {
            if(this.selectedRoomSO.maxRoomEnterance > roomEnterenceCount)
            {
                int randomise = Random.Range(0, 2);

                if (randomise > 0)
                {
                    // change selectedhallstartcell with new one
                    cell.AddHallStartRoomList(this);
                    selectedhallStartCell = cell;
                    roomEnterenceCount++;
                }
            }
            

            return selectedhallStartCell;

        }

    }

    public class SpawnAreaAndRotation
    {
        public Rect spawnArea;
        public Vector3 spawnRotation;
    }
    /*
    public SpawnAreaAndRotation GetWhereToSpawn(DungeonObjectSO dungeonObjectSO)
    {
        SpawnAreaAndRotation SAR = new SpawnAreaAndRotation();

        if(dungeonObjectSO.roomSpawnPoint == DungeonObjectSO.WhereToSpawn.all)
        {
            SAR.spawnArea = outerSpawnAreaOut;
        }
        if (dungeonObjectSO.roomSpawnPoint == DungeonObjectSO.WhereToSpawn.middle)
        {
            SAR.spawnArea = innerSpawnArea;
        }
        if (dungeonObjectSO.roomSpawnPoint == DungeonObjectSO.WhereToSpawn.edge)
        {
            int i = Random.Range(0, GetAllEdgesList().Count);
            SAR.spawnArea = GetAllEdgesList()[i];
            
            if(SAR.spawnArea == edgeW) SAR.spawnRotation = new Vector3(0, 90, 0);
            if(SAR.spawnArea == edgeE) SAR.spawnRotation = new Vector3(0, -90, 0);
            if(SAR.spawnArea == edgeN) SAR.spawnRotation = new Vector3(0, 180, 0);
            if(SAR.spawnArea == edgeS) SAR.spawnRotation = new Vector3(0, 0, 0);



        }
        if (dungeonObjectSO.roomSpawnPoint == DungeonObjectSO.WhereToSpawn.corner)
        {
            int i = Random.Range(0, GetAllCornersList().Count);
            SAR.spawnArea = GetAllCornersList()[i];
            
        }


        return SAR;
    }
    */
    
}