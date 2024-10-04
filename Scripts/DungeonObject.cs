using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonObject : MonoBehaviour//, IDamagable
{

    public int health;

    public List<Transform> spawnPoints;

    private void Awake()
    {
        DungeonCreator.OnAnySpawnerComplete += DungeonCreator_OnAnyRoomCreationComplete;
    }

    private void DungeonCreator_OnAnyRoomCreationComplete(object sender, EventArgs e)
    {
        Collider collider = GetComponent<Collider>();

        if (collider != null)
        {
            Destroy(collider);

        }
    }
    private void OnDestroy()
    {
        DungeonCreator.OnAnySpawnerComplete -= DungeonCreator_OnAnyRoomCreationComplete;
    }
    public void Damage(int damage)
    {
        health -= damage;
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        if (health < 0)
        {
            health = 0;
        }

        if (health == 0 )
        {
            Destroy(transform.gameObject);
        }
    }
}
