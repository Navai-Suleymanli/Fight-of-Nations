using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int enemyCount = 0;
    private const int maxEnemyCount = 1;
    private Vector3[] positions = new Vector3[maxEnemyCount];

    private void Awake()
    {
        // Assuming the barrier is aligned along its local Z-axis
        // and you want the enemies to be positioned along its local X-axis
        positions[0] = gameObject.transform.position + new Vector3(0f,0f,2.5f); // Position to the right of the barrier
        positions[1] = gameObject.transform.position - new Vector3(0f, 0f, 2.5f); ; // Position to the left of the barrier
    }

    public bool CanAddEnemy()
    {
        return enemyCount < maxEnemyCount;
    }

    public Vector3 GetPositionForEnemy()
    {
        if (CanAddEnemy())
        {
            // Assign the next position based on the current enemy count
            return positions[enemyCount];
        }
        else
        {
            Debug.LogError("Trying to add more enemies than allowed.");
            return Vector3.zero; // Return an invalid position
        }
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