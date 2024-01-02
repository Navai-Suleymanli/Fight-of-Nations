using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class PlayerManager : MonoBehaviour
{

    [SerializeField] private int playerHealth = 100;
    private AudioSource audioSource;
    [SerializeField] AudioClip playerAhhVoice;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        audioSource.PlayOneShot(playerAhhVoice, 1f);
        if (playerHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);        
    }
}
