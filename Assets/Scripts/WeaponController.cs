using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
    [Header("Shooting settings")]
    [SerializeField] float shootRange = 200;
    [SerializeField] float impact = 30.0f;
    [SerializeField] float fireRate = 19f;
    [SerializeField] float launchVelocity = 2000f;

    public GameObject impactEffect;
    public GameObject weaponHead;
    private Animator animator;
    private RecoilAndSway recoil_script;

    // ates effecti
    public Transform spawnPoint;
    public GameObject muzzle;

    // gulle gilizleri
    [SerializeField] GameObject bulletShell;
    [SerializeField] Transform spawnPoint2;




    private float nextTimeToShoot;

    [Header("Reload Stuff")]
    [SerializeField] int bulletCount = 30; // gulle sayi
    [SerializeField] bool isEmpty = false;
    [SerializeField] bool isReloading = false;

    [Header("Audio Stuff")]
    [SerializeField] AudioClip gunSound;
    [SerializeField] AudioClip emptyhot;

    [SerializeField] AudioClip reloadSound;
    AudioSource audioSource;

    [Header("bools")]
    [SerializeField] bool isSprinting;

    // empty code:
    [SerializeField] bool isAiming;




    private void Start()
    {
        InitializeAnimator();
        FindRecoilScript();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleShooting();
        HandleAiming();
        HandleReloading();

        isSprinting = Input.GetKey(KeyCode.LeftShift)?true:false;
        //Empty();
        isAiming = Input.GetKey(KeyCode.Mouse1) ? true : false;

    }

    
    private void InitializeAnimator()
    {
        animator = GetComponent<Animator>();
    }

    private void FindRecoilScript()
    {
        Transform mainCameraTransform = transform.Find("Main Camera");
        if (mainCameraTransform != null)
        {
            recoil_script = mainCameraTransform.GetComponent<RecoilAndSway>();
        }
        else
        {
            Debug.LogError("Could not find the MainCamera GameObject.");
        }
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToShoot && isEmpty == false && !isReloading && !isSprinting)
        {
            nextTimeToShoot = Time.time + 1f / fireRate;
            Shoot();
        }
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToShoot && isEmpty == false && !isReloading && isSprinting && isAiming)
        {
            nextTimeToShoot = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToShoot && isEmpty == true && !isReloading && !isSprinting)  //------------------
        {
            audioSource.PlayOneShot(emptyhot, 1f); 
        }


        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            animator.SetBool("shooting", false);
        }
    }




    private void HandleAiming()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            animator.SetBool("aiming", true);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            animator.SetBool("aiming", false);
        }
    }

    private void HandleReloading()
    {
        if (bulletCount <= 0)
        {
            isEmpty = true;
            animator.SetBool("shooting", false);
            Debug.Log("bullet finished!!!");
        }
        if (bulletCount == 30)
        {
            animator.SetBool("reload", false);
        }
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            animator.SetBool("shooting", false);
            Reload();
        }
    }

    private void Shoot()
    {
        // Prevent shooting when reloading
        if (isReloading) return;

        GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = weaponHead.transform.position;
            bullet.transform.rotation = weaponHead.transform.rotation;
            bullet.SetActive(true);

            bullet.GetComponent<bullet>().InitializeBullet(launchVelocity);

            GameObject currentMuzzle = Instantiate(muzzle, spawnPoint.transform.position, spawnPoint.transform.rotation);
            currentMuzzle.transform.parent = spawnPoint;





            GameObject currentBulletShell = Instantiate(bulletShell, spawnPoint2.transform.position, spawnPoint2.transform.rotation);
            currentBulletShell.transform.parent = spawnPoint2;





            audioSource.PlayOneShot(gunSound, 1f);
            bulletCount--;
        }

        ProcessRaycast();
        recoil_script.recoilFire();
        animator.SetBool("shooting", true);
    }

    private void Reload()
    {
        
        if (bulletCount < 30)
        {
            animator.SetBool("empty", false);
            isReloading = true;
            animator.SetBool("reload", true);
            StartCoroutine(reload());
            audioSource.PlayOneShot(reloadSound, 1f);
        }
    }

   /* private void Empty()
    {
        if(bulletCount <= 0 && Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToShoot && isEmpty == true && !isReloading && !isAiming)
        {
            animator.SetBool("empty", true );
            StartCoroutine(empty());    
        }
    }

    // ienumarator for empty:
    IEnumerator empty()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("empty", false);
    }
   */
    IEnumerator reload()
    {
        yield return new WaitForSeconds(2.7f);
        bulletCount = 30;
        isEmpty = false;
        isReloading = false;
        animator.SetBool("reload", false);  // Ensure shooting state is reset after reloading
    }

    private void ProcessRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(weaponHead.transform.position, weaponHead.transform.forward, out hit, shootRange))
        {
            Debug.Log(hit.transform.tag);
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impact);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);

            // Check if hit an enemy
            if (hit.transform.CompareTag("Enemy")) // Ensure your enemy GameObjects have the tag "Enemy"
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(20); // Decrease health by 20
                }
            }
        }
    }
}
