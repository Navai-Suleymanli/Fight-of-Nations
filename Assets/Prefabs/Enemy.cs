using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
   
    [SerializeField] float distance = 0.5f;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] bool isOccupation = false;
    private bool isDecreasing = false;



    // Enemy AI Stuff
    [SerializeField] float stopDistance = 5.0f; // Distance at which the enemy will stop moving towards the barrier

    public GameObject[] destinations = new GameObject[13]; // Array of all barriers



    private NavMeshAgent agent;

    private Barrier currentTargetBarrier = null;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToRandomDestination();     
    }

   


    void Update()
    {    
        isOccupation = Physics.CheckSphere(gameObject.transform.position, distance, groundLayer);

        if (isOccupation && !isDecreasing)
        {
            StartCoroutine(DecreaseValue());
        }

        // barrier limits


    }


    private void FixedUpdate()
    {
        if (currentTargetBarrier != null)
        {
            // Calculate distance to the current target barrier
            float distanceToCurrentBarrier = Vector3.Distance(transform.position, currentTargetBarrier.transform.position);

            // Check if we are close to the current target barrier
            if (distanceToCurrentBarrier <= stopDistance)
            {
                // We have reached the barrier, stop moving
                agent.isStopped = true;
            }
            else if (distanceToCurrentBarrier > stopDistance && agent.isStopped)
            {
                // We are no longer close to the barrier, we should start moving again
                agent.isStopped = false;
            }
        }
        else
        {
            // No current target, so find one
            GoToRandomDestination();
        }
    }



    public void GoToRandomDestination()
    {
        // If we already have a target, do nothing
        if (currentTargetBarrier != null) return;

        List<int> validDestinationIndices = new List<int>();

        // Check each barrier to see if it can accept more enemies
        for (int i = 0; i < destinations.Length; i++)
        {
            Barrier barrier = destinations[i].GetComponent<Barrier>();
            if (barrier != null && barrier.enemyCount < 2)
            {
                validDestinationIndices.Add(i);
            }
        }

        // If there are valid destinations that can accept more enemies, pick one at random
        if (validDestinationIndices.Count > 0)
        {
            int randomIndex = validDestinationIndices[Random.Range(0, validDestinationIndices.Count)];
            GameObject selectedDestination = destinations[randomIndex];
            agent.SetDestination(selectedDestination.transform.position);
            currentTargetBarrier = selectedDestination.GetComponent<Barrier>();
            // Increment the enemy count here to prevent other enemies from targeting this barrier
            currentTargetBarrier.AddEnemy();
            // Ensure the agent is not stopped so it can move to the destination
            agent.isStopped = false;
        }
        else
        {
            Debug.LogWarning("No available barrier with less than 2 enemies.");
            // Optionally, choose a different behavior when no barriers are available
        }
    }


    private IEnumerator DecreaseValue()
    {
        isDecreasing = true;

        while (OccupationManager.valueToDecrease > 0) 
        {
            OccupationManager.valueToDecrease -= 1;
            yield return new WaitForSeconds(2f); // Adjust time as necessary
        }

        isDecreasing = false;
    }
}
