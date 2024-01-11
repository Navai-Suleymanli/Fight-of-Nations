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

    [Header("Shooting at Player")]
    [SerializeField] private Vector3 realShootVector;
    [SerializeField] private Vector3 shootVector;
    [SerializeField] GameObject player;
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private int enemyDamage = 10;


    [Header("Health System")]
    [SerializeField] private int health = 100;

    [SerializeField] private float fireRate1 = 0.5f;
    [SerializeField] private float fireRate2 = 1f;
    private float nextTimeToShoot;

    [Header("Gun Shot Stuff")]
    AudioSource audioSource;
    [SerializeField] AudioClip[] gunShot;
    public GameObject impactEffect;
    [SerializeField] AudioClip impactSound;


    





    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GoToRandomDestination();
        audioSource = GetComponent<AudioSource>();
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
            float distanceToCurrentBarrier = Vector3.Distance(transform.position, currentTargetBarrier.transform.position); // calculating distace to the current barrier
            if (distanceToCurrentBarrier <= stopDistance)
            {
                agent.isStopped = true;

                // enemy shooting
                if (Time.time >= nextTimeToShoot)
                {
                    nextTimeToShoot = Time.time + 1f / Random.Range(fireRate1, fireRate2);
                    shootPlayer();
                }

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


    public void shootPlayer()
    {
        // Enemy Shooting stuff:
        realShootVector = player.transform.position - gameObject.transform.position;
        shootVector = realShootVector + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        Vector3 startPoint = gameObject.transform.position;
        Vector3 endPoint = startPoint + shootVector;
        RaycastHit hit; // raycast element

        if (Physics.Raycast(gameObject.transform.position, shootVector, out hit, shootRange) && hit.transform.tag == "Player")
        {
            Debug.Log(hit.transform.tag);

            PlayerManager player = hit.transform.GetComponent<PlayerManager>();
            if (player != null)
            {
                player.TakeDamage(enemyDamage);
            }
            // Draws a blue line from this transform to the target
            Debug.DrawLine(startPoint, endPoint, Color.green, 0.1f);

        }
        else
        {
            Debug.DrawLine(startPoint, endPoint, Color.red, 0.1f);

            GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGo, 2f);

            AudioSource.PlayClipAtPoint(impactSound, hit.point,0.5f);

        }
        int i = Random.Range(0, 5);
        AudioSource.PlayClipAtPoint(gunShot[i],gameObject.transform.position,1.0f);
        //audioSource.PlayOneShot(gunShot[i], 1f);
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

    // ---------------------------------------------------------------------------------------------- //
    private IEnumerator DecreaseValue()
    {
        isDecreasing = true;
        while (OccupationManager.EnemyPercentage < 100)
        {
            OccupationManager.EnemyPercentage += 1;
            yield return new WaitForSeconds(5f);
        }
        isDecreasing = false;
    }

    private void OnDestroy()
    {
        currentTargetBarrier?.RemoveEnemy();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle enemy death here (e.g., play animation, remove from game, etc.)
        gameObject.SetActive(false);
    }
}
