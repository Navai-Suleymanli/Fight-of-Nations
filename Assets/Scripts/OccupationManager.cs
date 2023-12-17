using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupationManager : MonoBehaviour
{
    public static int valueToDecrease = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(valueToDecrease);
        if (valueToDecrease <= 0)
        {
            Debug.Log("YOU LOST THE TERRITOYRY!!!");
        }
    }
}
