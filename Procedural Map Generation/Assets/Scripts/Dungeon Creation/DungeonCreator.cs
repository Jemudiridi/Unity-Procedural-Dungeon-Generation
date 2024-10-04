using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Utility.Diridi;
using static Cell;
using static Room;
using Random = UnityEngine.Random;

public class DungeonCreator : MonoBehaviour
{
    public static event EventHandler OnAnyDungeonMapComplete;

    public static event EventHandler OnAnySpawnerComplete;


    //[SerializeField] private NavMeshSurface navmesh;

    [Header("Prefabs")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject skeleton;
    [SerializeField] private GameObject undead;

    [Header("Map Size")]
    [SerializeField] int width;
    [SerializeField] int depth;
    [SerializeField] int cellVolume;
    [SerializeField] Vector3 dungeonStartPoint;
    [SerializeField] int minRoomCount;
    [SerializeField] int maxRoomCount;
    [SerializeField] Vector2Int minRoomSize;
    [SerializeField] Vector2Int maxRoomSize;

    [Header("Prefabs")]
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private LevelSO levelSO;


    private List<Cell> allCells;
    private List<Cell> activeCells;
    private List<Cell> collapsedCells;
    private List<Room> rooms;
    private List<GameObject> instantiatedGOList;
    private List<GameObject> spawnedCreaturesList;
    private Vector3 playerSpawnPoint;

    private float buildTimer = .1f;
    private bool buildTimerBool = false;

    
    


    private void Awake()
    {
        CreateDungeon();
        DestroyUnusedCells();
        //player.transform.position = playerSpawnPoint;

    }

    private void Update()
    {
        BuildTimerHandler();

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
            player.transform.position = playerSpawnPoint;
        }

    }

    private void BuildTimerHandler()
    {
        if (!buildTimerBool)
        {
            buildTimer -= Time.deltaTime;
        }

        if (buildTimer <= 0 && !buildTimerBool)
        {
            //navmesh.BuildNavMesh();
            //Spawncreatures();
            OnAnyDungeonMapComplete?.Invoke(this, EventArgs.Empty);

            buildTimerBool = true;

        }
    }

    /*
    private void Spawncreatures()
    {

        foreach(Room room in rooms)
        {
            if(room.selectedRoomSO == levelSO.basicRoom)
            {
                int i = (int)MathF.Sqrt(room.roomCells.Count);


                for(int y = 0; y < i; y++)
                {
                    int roomCellNumber = Random.Range(0, room.roomCells.Count);
                    Transform spawntransform = room.roomCells[roomCellNumber].spawnPoint;
                    int x = Random.Range(0, 2);

                    if(NavMesh.SamplePosition(spawntransform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    {

                        if (x > 0)
                        {
                           GameObject creature = Instantiate(skeleton, hit.position, Quaternion.identity);

                            spawnedCreaturesList.Add(creature);
                        }
                        else
                        {
                            GameObject creature = Instantiate(undead, hit.position, Quaternion.identity);

                            spawnedCreaturesList.Add(creature);

                        }

                    };

                    
                    
                }

                

            }
        }
    }
    */
    private void CreateDungeon()
    {
        allCells = new List<Cell>();
        activeCells = new List<Cell>();
        collapsedCells = new List<Cell>();
        rooms = new List<Room>();
        instantiatedGOList = new List<GameObject>();
        spawnedCreaturesList = new List<GameObject>();

        InitializeGrid();

        foreach (Cell cell in allCells)
        {
            TrySetAllNeighbours(cell);
        }


        PlaceRooms(levelSO);

        InitializeHalls();

        //CreateSpawnPoints();

        //SpawnAllObjectsInAllRooms();


        OnAnyDungeonMapComplete?.Invoke(this, EventArgs.Empty);

    }



    private void DestroyDungeon()
    {
        foreach (Cell cell in allCells)
        {
            Destroy(cell.gameObject);
        }
    }


    

    private void PlaceRooms(LevelSO levelSO)
    {
        int selectedRoomCount = Random.Range(minRoomCount, maxRoomCount);

        bool isStartRoomPlaced = false;
        bool isEndRoomPlaced = false;

        int tryCount = 999;
        while (rooms.Count < selectedRoomCount && tryCount > 0)
        {

            if (!isStartRoomPlaced) 
            {
                CreateRoomSquare(levelSO.startRoomSO.roomCellCount, levelSO.startRoomSO.roomCellCount, levelSO.startRoomSO);

                isStartRoomPlaced = true; 
            }

            if (!isEndRoomPlaced)
            {
                CreateRoomSquare(levelSO.endRoomSO.roomCellCount, levelSO.endRoomSO.roomCellCount, levelSO.endRoomSO);
                isEndRoomPlaced = true;
            }
            CreateRoomSquare(minRoomSize,maxRoomSize, levelSO.basicRoom);

            tryCount--;
        }

        // Collpase Rooms
        foreach (var room in rooms)
        {
            InitializeRoomCollapse(room);

        }


    }

    private void CreateRoomSquare(Vector2Int selectedMinRoomSize, Vector2Int selectedMaxRoomSize , RoomSO roomSO)
    {
        Vector2Int roomSize = new Vector2Int(
            Random.Range(selectedMinRoomSize.x, selectedMaxRoomSize.x) * cellVolume,
            Random.Range(selectedMinRoomSize.y, selectedMaxRoomSize.y) * cellVolume
        );
        Vector2Int location = new Vector2Int(
                Random.Range(0, width - roomSize.x / cellVolume) * cellVolume,
                Random.Range(0, depth - roomSize.y / cellVolume) * cellVolume
            );

        

        Room newRoom = new Room(location, roomSize, cellVolume);

        

        bool addRoom = true;

        if (newRoom.bounds.xMin < 0 || newRoom.bounds.width >= width * cellVolume
            || newRoom.bounds.yMin < 0 || newRoom.bounds.height >= depth * cellVolume)
        {
            addRoom = false;
        }

        if (rooms.Count > 0)
        {
            foreach (var room2 in rooms)
            {
                if (newRoom.bounds.Overlaps(room2.buffer))
                {
                    addRoom = false;
                }

            }
        }

        if (addRoom)
        {
            AddRoom(newRoom);
            DrawRect(newRoom.cornerNW);
            DrawRect(newRoom.cornerNE);
            DrawRect(newRoom.cornerSW);
            DrawRect(newRoom.cornerSE);
            DrawRect(newRoom.edgeN);
            DrawRect(newRoom.edgeE);
            DrawRect(newRoom.edgeS);
            DrawRect(newRoom.edgeW);
            newRoom.selectedRoomSO = roomSO;
            SetAllCellsOfRoom(newRoom);
        }
    }


   public void DrawRect(Rect rect)
    {
        Vector3 minx = new Vector3(rect.xMax, 5, rect.yMin);
        Vector3 maxx = new Vector3(rect.xMin, 5, rect.yMin);
        Vector3 maxy = new Vector3(rect.xMax, 5, rect.yMax);
        Vector3 miny = new Vector3(rect.xMin, 5, rect.yMax);
        Vector3 middleDOwn = new Vector3(rect.center.x, 0, rect.center.y);
        Vector3 middleup = new Vector3(rect.center.x, 15, rect.center.y);
        Debug.DrawLine(middleDOwn, middleup, UnityEngine.Color.white, 2f);

        Debug.DrawLine(minx, maxx, UnityEngine.Color.white, 2f);
        Debug.DrawLine(maxy, miny, UnityEngine.Color.white, 2f);
        Debug.DrawLine(maxy, maxx, UnityEngine.Color.white, 2f);
        Debug.DrawLine(minx, miny, UnityEngine.Color.white, 2f);


    }

    private void AddRoom(Room room)
    {
        rooms.Add(room);
        int roomCount = rooms.IndexOf(room);
        room.connectedRooms.Add(roomCount);
        room.roomNumber = roomCount;
    }

    private void InitializeRoomCollapse(Room room)
    {
        foreach (Cell cell in room.roomCells)
        {
            if (!cell.IsCollapsed())
            {
                cell.CollapseCell(Cell.CellType.Room);
                activeCells.Remove(cell);
                collapsedCells.Add(cell);
                GameObject roommaa = Instantiate(room.selectedRoomSO.roomPrefab, cell.transform);
                cell.collapsedobect = roommaa;
                cell.boundedRoom = room;
                cell.connectedRooms.Add(rooms.IndexOf(room));

            }


        }
    }

    public List<Cell> SetAllCellsOfRoom(Room room)
    {
        List<Cell> roomCells = new List<Cell>();
        foreach (Cell cell in allCells)
        {
            if ((cell.gameObject.transform.position.x >= room.bounds.xMin) && (cell.gameObject.transform.position.z >= room.bounds.yMin))
            {
                if ((cell.gameObject.transform.position.x <= room.bounds.xMax) && (cell.gameObject.transform.position.z <= room.bounds.yMax))
                {
                    roomCells.Add(cell);
                    room.roomCells.Add(cell);

                }
            }
        }
        return roomCells;
             
    }
   
    private void InitializeHalls()
    {
        List<Room> tempRooms = new List<Room>();

        Room roomStart = null;
        Room roomEnd = null;
        int i = 2;

        foreach (Room room in rooms)
        {
            room.SetPossibleHallStartCells();
            if (room.connectedRooms.Count < i)
            {
                tempRooms.Add(room);
            }
        }

        while (i < rooms.Count)
        {
            if (tempRooms.Count == 0)
            {
                foreach (Room room in rooms)
                {
                    if (room.connectedRooms.Count < i)
                    {
                        tempRooms.Add(room);
                    }
                }
                i++;
            }

            if (tempRooms.Count == 1)
            {
                roomStart = tempRooms[0];

                List<Room> tempRooms2 = new List<Room>();
                tempRooms2.AddRange(rooms);
                tempRooms2.Remove(roomStart);
                roomEnd = ChooseRandomRoom(tempRooms2);
            }


            if (tempRooms.Count > 1)
            {
                roomStart = ChooseRandomRoom(tempRooms);
                roomEnd = ChooseRandomRoom(tempRooms);
            }

            foreach (int roomNumber in roomEnd.connectedRooms)
            {
                if (!roomStart.connectedRooms.Contains(roomNumber))
                {
                    roomStart.connectedRooms.Add(roomNumber);

                    foreach (Cell cell in roomStart.roomCells)
                    {
                        cell.connectedRooms.Add(roomNumber);
                    }
                }
            }

            foreach (int roomNumber in roomStart.connectedRooms)
            {
                if (!roomEnd.connectedRooms.Contains(roomNumber))
                {
                    roomEnd.connectedRooms.Add(roomNumber);

                    foreach (Cell cell in roomEnd.roomCells)
                    {
                        cell.connectedRooms.Add(roomNumber);
                    }
                }
            }


            Cell startCell = roomStart.SelectHallStartCell();
            Cell endCell = roomEnd.SelectHallStartCell();

            startCell.isEnterance = true;

            endCell.isEnterance = true;


            foreach (Cell cell in PathfindHall(startCell, endCell))
            {
                if (cell.GetCellType() == Cell.CellType.Empty)
                {
                    cell.CollapseCell(Cell.CellType.Hall);
                    GameObject hall = Instantiate(levelSO.hallRoom.roomPrefab, cell.transform);
                    cell.collapsedobect = hall;

                    
                }


                cell.connectedRooms.Add(rooms.IndexOf(roomStart));
                cell.connectedRooms.Add(rooms.IndexOf(roomEnd));
            }

            if (roomStart.connectedRooms.Count == rooms.Count) break;
            if (roomEnd.connectedRooms.Count == rooms.Count) break;

        }
    }

    private void CreateSpawnPoints()
    {

        List<Cell> randomCells = new List<Cell>();

        foreach (Cell cell in allCells)
        {
            if (cell.GetCellType() == CellType.Hall)
            {
                randomCells.Add(cell);
            }
        }
       

        playerSpawnPoint = rooms[0].roomCells[0].spawnPoint.position;
        }

    private Room ChooseRandomRoom(List<Room> rooms)
    {
        int randomIndex = Random.Range(0, rooms.Count);
        Room room = rooms[randomIndex];
        rooms.Remove(room);
        return room;
    }

    private List<Cell> PathfindHall(Cell startCell, Cell endCell)
    {
        List<Cell> opencells = new List<Cell> {startCell};
        List<Cell> closedCells = new List<Cell>();

        foreach (var cell in activeCells)
        {
            cell.gCost = int.MaxValue;
            cell.CalculateFCost();
            cell.cameFromCell = null;
        }

        startCell.gCost = 0;
        startCell.hCost = CalculateDistance(startCell, endCell);
        startCell.CalculateFCost();

        while (opencells.Count > 0)
        {
            Cell currentCell = GetClosestCell(opencells);

            if (currentCell == endCell) 
            {
                // Reached final cell
                return CalculatePath(endCell);
            }


            opencells.Remove(currentCell);
            closedCells.Add(currentCell);

            foreach (Cell neighbourCell in currentCell.GetAllNeighboursList())
            {
                if (closedCells.Contains(neighbourCell)) continue;

                int tentativeGCost;
                if (neighbourCell.GetCellType() == Cell.CellType.Hall)
                {
                    tentativeGCost = currentCell.gCost + 2 * CalculateDistance(currentCell, neighbourCell);

                } else
                {
                    tentativeGCost = currentCell.gCost + 5 * CalculateDistance(currentCell, neighbourCell);
                }
                if (tentativeGCost < neighbourCell.gCost) 
                {
                    neighbourCell.cameFromCell = currentCell;
                    neighbourCell.gCost = tentativeGCost;
                    neighbourCell.hCost = CalculateDistance(neighbourCell, endCell);
                    neighbourCell.CalculateFCost();

                    if (!opencells.Contains(neighbourCell))
                    {
                        opencells.Add(neighbourCell);
                    }
                }
            }
        }

        // Out of nodes on the openCells
        return null;
    }

    private List<Cell> CalculatePath(Cell endCell)
    {
        List<Cell> path = new List<Cell>();
        path.Add(endCell);

        Cell currentCell = endCell;
        while (currentCell.cameFromCell != null)
        {
            path.Add(currentCell.cameFromCell);
            currentCell = currentCell.cameFromCell;
        }
        path.Reverse();
        return path;
    }

    private Cell GetClosestCell(List<Cell> cellList)
    {
        Cell closestCell = cellList[0];
        for (int i = 1; i < cellList.Count; i++)
        {
            if (cellList[i].fCost < closestCell.fCost)
            {
                closestCell = cellList[i];
            }
        }
        return closestCell;
    }

    private int CalculateDistance(Cell a, Cell b)
    {
        int xDistance = Mathf.Abs(a.gridPosition.x + b.gridPosition.x);
        int yDistance = Mathf.Abs(a.gridPosition.y + b.gridPosition.y);
        int distance = Mathf.Abs(xDistance + yDistance);
        return distance;
    }

    

    private void InitializeGrid()
    {
       for (int x = 0; x < width; x++)
       {
            for (int z  = 0; z < depth; z++)
            {

                CreateCell(false, new Vector2Int(x,z));
            }
       }
    }


    private void CreateCell(bool isCollapsed, Vector2Int pos)
    {
        Cell newCell = Instantiate(cellPrefab, new Vector3(pos.x * cellVolume, 0, pos.y * cellVolume) + new Vector3(cellVolume * .5f, 0, cellVolume * .5f), Quaternion.identity);
        newCell.SetCollapsed(isCollapsed);
        newCell.gridPosition = pos;

        if (!activeCells.Contains(newCell)) activeCells.Add(newCell);
        if (!allCells.Contains(newCell)) allCells.Add(newCell);
        
    }

    private Cell GetCell(float x, float z)
    {
        Cell cell = null;
        foreach (Cell cell2 in allCells)
        {
            if (cell2.transform.position == new Vector3(x, 0, z))
            {
                cell = cell2;
                break;
            } else
            {
                cell = null;
            }
        }
        return cell;
    }


    private void TrySetAllNeighbours(Cell cell)
    {
        if (GetCell(cell.transform.position.x + 0, cell.transform.position.z + cellVolume) != null)
        {
            cell.SetNorthNeighbour(GetCell(cell.transform.position.x + 0, cell.transform.position.z + cellVolume));
        }

        if (GetCell(cell.transform.position.x + cellVolume, cell.transform.position.z + 0) != null)
        {
            cell.SetEastNeighbour(GetCell(cell.transform.position.x + cellVolume, cell.transform.position.z + 0));
        }

        if (GetCell(cell.transform.position.x + 0, cell.transform.position.z - cellVolume) != null)
        {
            cell.SetSouthNeighbour(GetCell(cell.transform.position.x + 0, cell.transform.position.z - cellVolume));
        }

        if (GetCell(cell.transform.position.x - cellVolume, cell.transform.position.z + 0) != null)
        {
            cell.SetWestNeighbour(GetCell(cell.transform.position.x - cellVolume, cell.transform.position.z + 0));
        }

    }

    
    private void DestroyUnusedCells()
    {
        List<Cell> cellsToDestroy = new List<Cell>();
        foreach(Cell cell in allCells)
        {
            if (cell.IsCollapsed() == false)
            {
                cellsToDestroy.Add(cell);
            }
        }
        foreach(Cell cell in cellsToDestroy)
        {
            allCells.Remove(cell);

            Destroy(cell.gameObject);
        }

    }

    private void DestroyUnusedObjects()
    {
        
        foreach (GameObject spawnedObject in instantiatedGOList)
        {

            Destroy(spawnedObject);
        }

        foreach ( GameObject spawnedCreature in spawnedCreaturesList)
        {
            Destroy(spawnedCreature);
        }
    }



    /*

    private void SpawnAllObjectsInAllRooms()
    {
        
        StartCoroutine(ISpawnTest());

    }


    public IEnumerator ISpawnTest()
    {
        foreach (Room room in rooms) 
        {
            foreach (DungeonObjectSpawnPackageforRoom objectSpawnPackage in room.selectedRoomSO.objectSpawnPackages)
            {

                DungeonObjectSO dungeonObjectSO = objectSpawnPackage.spawnableObjectSO;


                int tryCount = 0;
                int selectedSpawnCount;
                if (!room.selectedRoomSO.isConstantObjectCount)
                {
                    int iMax = (int)MathF.Round(objectSpawnPackage.maxSpawn * MathF.Sqrt(room.roomCells.Count));
                    int iMin = objectSpawnPackage.minSpawn;

                    selectedSpawnCount = Random.Range(iMin, iMax + 1);
                }
                else
                {
                    selectedSpawnCount = objectSpawnPackage.minSpawn;
                }



                List<DungeonObjectSO> instantiatedDungeonObjectSOs = new List<DungeonObjectSO>();

                // yield return null;


                while (instantiatedDungeonObjectSOs.Count < selectedSpawnCount && tryCount < 999)
                {

                    //yield return null;

                    SpawnAreaAndRotation spawnAnR = room.GetWhereToSpawn(dungeonObjectSO);
                    Rect spawnArea = spawnAnR.spawnArea;
                    Vector3 spawnRotation = spawnAnR.spawnRotation;
                    float positionX = Random.Range(spawnArea.xMin, spawnArea.xMax);
                    float positionZ = Random.Range(spawnArea.yMin, spawnArea.yMax);

                    Vector3 spawnLocation = new Vector3(positionX, 1.5f, positionZ);

                    CollisionTest collisionTest = CollisionTest(dungeonObjectSO.objectPrefab, spawnLocation, spawnRotation);

                    yield return null;

                    if (!collisionTest.hittedAnObject)
                    {
                        GameObject instantiatedObject = Instantiate(dungeonObjectSO.objectPrefab, spawnLocation, Quaternion.Euler(spawnRotation));
                        instantiatedGOList.Add(instantiatedObject);


                        SpawnDungeonItemsPerRoom(instantiatedObject.GetComponent<DungeonObject>().spawnPoints, room);

                        instantiatedDungeonObjectSOs.Add(dungeonObjectSO);
                    }

                    Destroy(collisionTest.gameObject);


                    tryCount++;
                }
                //yield return null;
                OnAnyDungeonMapComplete?.Invoke(this, EventArgs.Empty);

            }
        }
        yield return null;

        OnAnySpawnerComplete?.Invoke(this, EventArgs.Empty);
    }

    private CollisionTest CollisionTest(GameObject spawnTestObject, Vector3 spawnLocation, Vector3 spawnRotation)
    {
        Bounds bounds = UtilityDungeon.GetBounds(spawnTestObject);



        GameObject collisionTestObject = new GameObject();
        BoxCollider testCollider = collisionTestObject.AddComponent<BoxCollider>();

        testCollider.isTrigger = true;
        testCollider.enabled = true;
        testCollider.size = bounds.size + new Vector3( 1f, 1f, 1f);



        CollisionTest collisionTest = collisionTestObject.AddComponent<CollisionTest>();


        Rigidbody rb = collisionTestObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        collisionTestObject.name = "CollisionTest";
        collisionTestObject.transform.position = spawnLocation;
        collisionTestObject.transform.rotation = Quaternion.Euler(spawnRotation);

        return collisionTest;


    }


    private void SpawnDungeonItemsPerRoom(List<Transform> SpawnPointList, Room room)
    {
        int spawnTimes = 2;

        for(int z = 0; z < spawnTimes; z++)
        {
            if(SpawnPointList.Count > 0)
            {
                int i = Random.Range(0, SpawnPointList.Count);

                Transform selectedSpawnPoint = SpawnPointList[i];

                if (selectedSpawnPoint != null)
                {
                    int x = Random.Range(0, room.selectedRoomSO.spawnableObjectSO.Count);
                    ItemSO selectedItemSO = room.selectedRoomSO.spawnableObjectSO[x];


                    GameObject instantiatedObject = Instantiate(selectedItemSO.itemPrefab, selectedSpawnPoint.position, Quaternion.Euler(new Vector3(90, 0, 0)));

                    instantiatedGOList.Add(instantiatedObject);
                }
            }
            
        }

        
    }

    */
}
