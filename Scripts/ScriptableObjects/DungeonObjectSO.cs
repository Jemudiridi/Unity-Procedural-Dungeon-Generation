using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DungeonObjectSO : ScriptableObject
{
    public enum WhereToSpawn
    {
        all,
        middle,
        edge,
        corner,
    }

    public string objectName;
    public GameObject objectPrefab;
    public WhereToSpawn roomSpawnPoint;
    public Quaternion spawnRotationChangeDegree;
}
