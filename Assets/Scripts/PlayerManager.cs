using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{

    
    private AudioSource audioSource;
    [SerializeField] AudioClip playerAhhVoice;
    public Image healthBar;
    [SerializeField] private int playerHealth = 100;


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
        healthBar.fillAmount = playerHealth / 100f;
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
