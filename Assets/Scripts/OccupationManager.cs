using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OccupationManager : MonoBehaviour
{
    public static int EnemyPercentage = 0;
    public Image enemyTakenArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(EnemyPercentage);
        enemyTakenArea.fillAmount = EnemyPercentage / 100f;
        if (EnemyPercentage >= 100)
        {
            Debug.Log("YOU LOST THE TERRITOYRY!!!");
        }
    }
}
