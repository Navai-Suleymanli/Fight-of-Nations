using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int enemyCount = 0;
    private const int maxEnemyCount = 2;

    public bool CanAddEnemy()
    {
        return enemyCount < maxEnemyCount;
    }

    public void AddEnemy()
    {
        if (CanAddEnemy())
        {
            enemyCount++;
        }
    }

    public void RemoveEnemy()
    {
        if (enemyCount > 0)
        {
            enemyCount--;
        }
    }
}

