using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomSO : ScriptableObject
{
    public string roomName;

    public bool isConstantObjectCount = false;

    public Vector2Int roomCellCount;
    public Vector2Int roomOffset;
    public GameObject roomPrefab;


    public int maxRoomEnterance = 3;

    public List<DungeonObjectSpawnPackageforRoom> objectSpawnPackages;

    public List<ItemSO> spawnableObjectSO;



}

[System.Serializable]
public class DungeonObjectSpawnPackageforRoom
{
    public DungeonObjectSO spawnableObjectSO;
    public int maxSpawn;
    public int minSpawn;

}
