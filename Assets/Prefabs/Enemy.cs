using System.Collections;
using System.Collections.Generic;
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
        isOccupation = Physics.CheckSphere(transform.position, distance, groundLayer);
        if (isOccupation && !isDecreasing)
        {
            StartCoroutine(DecreaseValue());
        }
    }

    private void FixedUpdate()
    {
        if (currentTargetBarrier != null)
        {
            float distanceToCurrentBarrier = Vector3.Distance(transform.position, currentTargetBarrier.transform.position);
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
            GoToRandomDestination();
        }
    }

    private void GoToRandomDestination()
    {
        if (currentTargetBarrier != null && currentTargetBarrier.enemyCount < 2) return;

        List<Barrier> validBarriers = new List<Barrier>();
        foreach (GameObject destination in destinations)
        {
            Barrier barrier = destination.GetComponent<Barrier>();
            if (barrier != null && barrier.CanAddEnemy())
            {
                validBarriers.Add(barrier);
            }
        }

        if (validBarriers.Count > 0)
        {
            Barrier selectedBarrier = validBarriers[Random.Range(0, validBarriers.Count)];
            agent.SetDestination(selectedBarrier.transform.position);
            currentTargetBarrier?.RemoveEnemy(); // Remove from old barrier
            currentTargetBarrier = selectedBarrier;
            currentTargetBarrier.AddEnemy();
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
