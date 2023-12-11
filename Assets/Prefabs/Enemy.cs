using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Occupation System")]
    [SerializeField] private float distance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isOccupation = false;
    private bool isDecreasing = false;

    [Header("AI System")]
    [SerializeField] private float stopDistance = 5.0f;
    public GameObject[] destinations;

    private NavMeshAgent agent;
    private Barrier currentTargetBarrier = null;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToRandomDestination();
    }

    void Update()
    {
        // occupation ststem  ----------------------------------
        isOccupation = Physics.CheckSphere(transform.position, distance, groundLayer);
        if (isOccupation && !isDecreasing)
        {
            StartCoroutine(DecreaseValue());
        }
        // -----------------------------------------------------
    }

    private void FixedUpdate() 
    {
        if (currentTargetBarrier != null) // if currenttarget barrier is defined(not null)
        {
            float distanceToCurrentBarrier = Vector3.Distance(transform.position, currentTargetBarrier.transform.position); // calculating distace to teh current barrier
            if (distanceToCurrentBarrier <= stopDistance)
            {
                agent.isStopped = true;
            }
            else if (distanceToCurrentBarrier > stopDistance)
            {
                agent.isStopped = false;
            }
        }
        else
        {
            GoToRandomDestination();  //this will set a barrier to the currentactive barrier. that abrrier will be chosen randomly from the list of active barriers
        }
    }

    private void GoToRandomDestination()
    {
        if (currentTargetBarrier != null && currentTargetBarrier.enemyCount < 2) return;  // if curretn barrer has place to go, then do nothing

        List<Barrier> validBarriers = new List<Barrier>(); // list of valid(not full) barriers
        foreach (GameObject destination in destinations)
        {
            Barrier barrier = destination.GetComponent<Barrier>(); // getting individual abrrier component of each barrier in the array
            if (barrier != null && barrier.CanAddEnemy())  // if the chosen barrier has a barrier script, and is not full
            {
                validBarriers.Add(barrier);  // add that barrier to the list of valid barriers
            }
        }

        if (validBarriers.Count > 0)  // as long as there are valid barrier(s)
        {
            Barrier selectedBarrier = validBarriers[Random.Range(0, validBarriers.Count)]; // rondomly choose a barrier from the list of valid barriers
            agent.SetDestination(selectedBarrier.transform.position);
            currentTargetBarrier?.RemoveEnemy(); // Remove from old barrier when enemy changes the destination. 
            //If you don't remove the enemy from the currentTargetBarrier, the enemy might be counted as being at two barriers at once. 

            currentTargetBarrier = selectedBarrier;
            currentTargetBarrier.AddEnemy();  // add enemy safely when chosen the current barrier
            agent.isStopped = false;
        }
        else
        {
            Debug.LogWarning("No available barrier with less than 2 enemies.");
        }
    }

    private IEnumerator DecreaseValue()
    {
        isDecreasing = true;
        while (OccupationManager.valueToDecrease > 0)
        {
            OccupationManager.valueToDecrease -= 1;
            yield return new WaitForSeconds(2f);
        }
        isDecreasing = false;
    }

    private void OnDestroy()
    {
        currentTargetBarrier?.RemoveEnemy();
    }
}
