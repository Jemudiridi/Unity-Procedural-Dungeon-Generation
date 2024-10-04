using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelSO : ScriptableObject
{
    public string levelName;

    public RoomSO startRoomSO;
    public RoomSO endRoomSO;
    public RoomSO hallRoom;
    public RoomSO basicRoom;

}
