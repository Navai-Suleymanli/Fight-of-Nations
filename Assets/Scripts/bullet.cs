using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour
{
    private bool coroutineRunning = false;


   /* void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("wall"))
        {
            gameObject.SetActive(false);
        }
    }*/
    public void InitializeBullet(float launchVelocity)
    {
        if (!coroutineRunning)
        {
            StartCoroutine(DestroyBullet());
            coroutineRunning = true;
        }

        GetComponent<Rigidbody>().velocity = Vector3.zero; // Reset velocity if reusing bullets
        GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, launchVelocity));
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        coroutineRunning = false;
    }
}
