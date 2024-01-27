using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Rendering.PostProcessing;

public class WeaponController : MonoBehaviour
{
    [Header("Shooting settings")]
    [SerializeField] float shootRange = 200;
    [SerializeField] float impact = 30.0f;
    [SerializeField] float fireRate = 19f;
    [SerializeField] float fireRateSniper = 5f;
    [SerializeField] float launchVelocity = 2000f;

    public GameObject impactEffect;
    public GameObject impactEffectSniper;

    public GameObject weaponHead;
    public GameObject sniperHead;
    private Animator animator;
    private RecoilAndSway recoil_script;

    // shooting effects
    public Transform spawnPoint;
    public Transform sniperSpawnPoint;
    public GameObject muzzle;  // ------------------------------------------------------------------------
    public GameObject muzzleSniper;
    [SerializeField] GameObject[] muzzles;
    public GameObject blood;

    // bullet gilizleri
    [SerializeField] GameObject bulletShell;  // --------------------------------------------------------
    [SerializeField] GameObject bulletShellSniper;
    [SerializeField] Transform spawnPoint2;
    public Transform sniperSpawnPoint2;

    //rotation
    [Header("Weapon Rotation")]
    [SerializeField] private Transform weaponTransform;

    [Header("crosshair")]
    [SerializeField] private RawImage cross;
    Rect rect;
    float width;
    float height;

    public Camera mainCamera; // Reference to the main camera in the scene
    public RectTransform xHitEffectUI; // Assuming it's a RectTransform (like for an Image or Text)



    private float nextTimeToShoot;

    [Header("Reload Stuff")]
    [SerializeField] int bulletCount = 30; // gulle sayi
    [SerializeField] int bulletCountSniper = 10;
    [SerializeField] bool isEmpty = false;
    [SerializeField] bool isEmptySniper = false;
    public bool isReloading = false;
    [SerializeField] TextMeshProUGUI bulletCountText;
    public Image bullet3;
    public Image bullet2;
    public Image bullet1;

    [Header("Audio Stuff")]
    [SerializeField] AudioClip gunSound;
    [SerializeField] AudioClip emptyhot;
    [SerializeField] AudioClip gunSoundSniper;

    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip reloadSoundSniper;
    AudioSource audioSource;

    [Header("bools")]
    [SerializeField] bool isSprinting;
    [SerializeField] bool isMoving;

    public  bool Sniper =false;
    [SerializeField] bool AK47 = true;


    [Header("Weapons")]

    public GameObject AKM;
    public GameObject SniperRifle;





    // empty code:
    public bool isAiming;
    Combined combined;

    // light
    public GameObject pointLight;
    public GameObject pointLightSniper;


    [Header ("Post Processing Effects")]
    public PostProcessVolume volume;
    private DepthOfField depthOfField;
    private AmbientOcclusion ambientOcclusion;

    private void Start()
    {
        InitializeAnimator();
        //FindWeaponTransform();
        audioSource = GetComponent<AudioSource>();
        combined = GetComponent<Combined>();
        if(volume.profile.TryGetSettings(out depthOfField) &&
            volume.profile.TryGetSettings(out ambientOcclusion))
        {
            SetEffectsActive(true);
        }
    }

    private void LateUpdate()
    {
        HandleShooting();
        HandleAiming();
        HandleReloading();
        getImageSize();


        isSprinting = Input.GetKey(KeyCode.LeftShift) ? true : false;
        isMoving = Input.GetKey(KeyCode.W) ? true : false;
        
        //Empty();
        isAiming = Input.GetKey(KeyCode.Mouse1) ? true : false;

        if (AK47)
        {
            bulletCountText.text = bulletCount.ToString() + "/30";
        }
        else if (Sniper)
        {
            bulletCountText.text = bulletCountSniper.ToString() + "/10";
            if (isAiming)
            {
                DisableEffects();
            }
            else if (!isAiming)
            {
                SetEffectsActive(true);
            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Sniper = true;
            AK47 = false;
            AKM.SetActive(false);
            SniperRifle.SetActive(true);   
            animator.SetBool("Sniper", true);
            animator.SetBool("AK47", false);
            muzzles = GameObject.FindGameObjectsWithTag("Effects");
   
            for (int i = 0; i < muzzles.Length; i++)
            {
                Destroy(muzzles[i]);
                //muzzles[i].(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Sniper = false;
            AK47 = true;
            animator.SetBool("Sniper", false);
            animator.SetBool("AK47", true);
            AKM.SetActive(true);
            SniperRifle.SetActive(false);
            muzzles = GameObject.FindGameObjectsWithTag("Effects");
            for (int i = 0; i < muzzles.Length; i++)
            {
                //muzzles[i].SetActive(false);
                Destroy(muzzles[i]);
            }
        }

    }

    public void getImageSize()
    {
        rect = cross.rectTransform.rect;
        width = rect.width;
        height = rect.height;

    }
    public void setImageSize()
    {
        if (width < 100 && height < 100)
        {
            cross.rectTransform.sizeDelta = new Vector2(width += 10f, height += 10f);

        }

    }
    public void resetImageSize()
    {
        while (height > 50f && width > 50f)
        {
            cross.rectTransform.sizeDelta = new Vector2(width -= 10f, height -= 10f);
        }
    }

    private void InitializeAnimator()
    {
        animator = GetComponent<Animator>();
    }
    private void HandleAiming()
    {
        if (Sniper)
        {
            cross.gameObject.SetActive(false);
        }
        else 
        {
            cross.gameObject.SetActive(true);
        }


        if (Input.GetKey(KeyCode.Mouse1))
        {
            animator.SetBool("aiming", true);
            cross.gameObject.SetActive(false);

        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            animator.SetBool("aiming", false);
            cross.gameObject.SetActive(true);
        }
    }


    private void HandleShooting()
    {
        if (AK47)
        {
            // Updated logic: Can't shoot if (sprinting and moving) or reloading, unless aiming.
            bool canNotShoot = (isSprinting && isMoving || isReloading) && !isAiming;

            if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToShoot && !isEmpty && !canNotShoot)
            {
                nextTimeToShoot = Time.time + 1f / fireRate;
                Shoot();
                animator.SetBool("shooting", true);
            }

            /*if (Input.GetKeyDown(KeyCode.Mouse0) && !isEmpty && !canNotShoot)
            {
                Shoot();
                animator.SetBool("shooting", true);
            }*/
            else if (Input.GetKeyUp(KeyCode.Mouse0) || canNotShoot || isEmpty)
            {
                animator.SetBool("shooting", false);
                combined.Dayandir();
                resetImageSize();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && isEmpty && !isReloading)
            {
                AudioSource.PlayClipAtPoint(emptyhot, gameObject.transform.position, 1f);
                //audioSource.PlayOneShot(emptyhot, 1f);
            }
        }
        else if (Sniper)
        {
            // Updated logic: Can't shoot if (sprinting and moving) or reloading, unless aiming.
            bool canNotShoot = (isSprinting && isMoving || isReloading) && !isAiming;

            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToShoot && !isEmptySniper && !canNotShoot)
            {
                nextTimeToShoot = Time.time + 1f / fireRateSniper;
                Shoot();
                if (isAiming)
                {
                    StartCoroutine(StopRecoil2());
                }
                if (!isAiming)
                {
                    StartCoroutine(StopRecoil());
                }
                
                animator.SetBool("shooting", true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0) || canNotShoot || isEmptySniper)
            {
                animator.SetBool("shooting", false);
                //combined.Dayandir();
                if (isAiming)
                {
                    StartCoroutine(StopRecoil2());
                }
                if (!isAiming)
                {
                    StartCoroutine(StopRecoil());
                }
                resetImageSize();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && isEmptySniper && !isReloading)
            {
                AudioSource.PlayClipAtPoint(emptyhot, gameObject.transform.position, 1f);
                //audioSource.PlayOneShot(emptyhot, 1f);
            }

            
        }
    }
    public void SetEffectsActive(bool isActive)
    {
        depthOfField.active = isActive;
        ambientOcclusion.active = isActive;
    }


    public void DisableEffects()
    {
        SetEffectsActive(false);
    }


    private void Shoot()
    {
        if (AK47 && !Sniper)
        {
            // Prevent shooting when reloading
            if (isReloading) return;

            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
            if (bullet != null && bulletCount > 0)
            {
                bullet.transform.position = weaponHead.transform.position;
                bullet.transform.rotation = weaponHead.transform.rotation;
                bullet.SetActive(true);

                bullet.GetComponent<bullet>().InitializeBullet(launchVelocity);

                GameObject currentMuzzle = Instantiate(muzzle, spawnPoint.transform.position, spawnPoint.transform.rotation);
                currentMuzzle.transform.parent = spawnPoint;





                GameObject currentBulletShell = Instantiate(bulletShell, spawnPoint2.transform.position, spawnPoint2.transform.rotation);
                currentBulletShell.transform.parent = spawnPoint2;




                AudioSource.PlayClipAtPoint(gunSound, gameObject.transform.position, 0.2f);
                //audioSource.PlayOneShot(gunSound, 1f);
                bulletCount--;
                pointLight.gameObject.SetActive(true);
                StartCoroutine(LightBlyat());
                combined.TriggerRecoil();
            }

            ProcessRaycast();

            //animator.SetBool("shooting", true);
            setImageSize();
        }
        else if (Sniper && !AK47)
        {
            // Prevent shooting when reloading
            if (isReloading) return;

            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
            if (bullet != null && bulletCountSniper > 0)
            {
                bullet.transform.position = sniperHead.transform.position;
                bullet.transform.rotation = sniperHead.transform.rotation;
                bullet.SetActive(true);

                bullet.GetComponent<bullet>().InitializeBullet(launchVelocity);

                GameObject currentMuzzle = Instantiate(muzzleSniper, sniperSpawnPoint.transform.position, sniperSpawnPoint.transform.rotation);
                currentMuzzle.transform.parent = sniperSpawnPoint;





                GameObject currentBulletShell = Instantiate(bulletShellSniper, sniperSpawnPoint2.transform.position, sniperSpawnPoint2.transform.rotation);
                currentBulletShell.transform.parent = sniperSpawnPoint2;




                AudioSource.PlayClipAtPoint(gunSoundSniper, gameObject.transform.position, 0.4f);
                //audioSource.PlayOneShot(gunSound, 1f);
                bulletCountSniper--;
                pointLightSniper.gameObject.SetActive(true);
                StartCoroutine(LightBlyatSniper());
                combined.TriggerRecoil();
            }

            ProcessRaycast();

            setImageSize();
        }
        

    }

    IEnumerator LightBlyat()
    {
        yield return new WaitForSeconds(0.1f);
        pointLight.gameObject.SetActive(false);
    }

    IEnumerator LightBlyatSniper()
    {
        yield return new WaitForSeconds(0.1f);
        pointLightSniper.gameObject.SetActive(false);
    }
    private void HandleReloading()
    {
        if (AK47)
        {
            if (bulletCount == 0)
            {
                isEmpty = true;
                //animator.SetBool("shooting", false);
                Debug.Log("bullet finished!!!");
                StartCoroutine(NotStopShootingWhenOne()); ;
                bullet3.color = new Color(255, 255, 255, 0.5f);
                bullet2.color = new Color(255, 255, 255, 0.5f);
                bullet1.color = new Color(255, 255, 255, 0.5f);
            }
            if (bulletCount == 30)
            {
                animator.SetBool("reload", false);
                bullet3.color = new Color(255, 255, 255, 1);
                bullet2.color = new Color(255, 255, 255, 1);
                bullet1.color = new Color(255, 255, 255, 1);
            }
            if (bulletCount == 20)
            {
                bullet3.color = new Color(255, 255, 255, 0.5f);
                bullet2.color = new Color(255, 255, 255, 1);
                bullet1.color = new Color(255, 255, 255, 1);
            }
            if (bulletCount == 10)
            {
                bullet3.color = new Color(255, 255, 255, 0.5f);
                bullet2.color = new Color(255, 255, 255, 0.5f);
                bullet1.color = new Color(255, 255, 255, 1);
            }
        }
        else if (Sniper)
        {
            if (bulletCountSniper == 0)
            {
                isEmptySniper = true;
                //animator.SetBool("shooting", false);
                Debug.Log("bullet finished!!!");
                StartCoroutine(NotStopShootingWhenOne()); ;
                bullet3.color = new Color(255, 255, 255, 0.5f);
                bullet2.color = new Color(255, 255, 255, 0.5f);
                bullet1.color = new Color(255, 255, 255, 0.5f);
            }
            if (bulletCountSniper == 10)
            {
                animator.SetBool("reload", false);
                bullet3.color = new Color(255, 255, 255, 1);
                bullet2.color = new Color(255, 255, 255, 1);
                bullet1.color = new Color(255, 255, 255, 1);
            }
            if (bulletCount == 6)
            {
                bullet3.color = new Color(255, 255, 255, 0.5f);
                bullet2.color = new Color(255, 255, 255, 1);
                bullet1.color = new Color(255, 255, 255, 1);
            }
            if (bulletCount == 2)
            {
                bullet3.color = new Color(255, 255, 255, 0.5f);
                bullet2.color = new Color(255, 255, 255, 0.5f);
                bullet1.color = new Color(255, 255, 255, 1);
            }
        }


        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            animator.SetBool("shooting", false);
            Reload();
        }
    }

    IEnumerator StopRecoil()
    {
        yield return new WaitForSeconds(0.1f);
        combined.Dayandir();
    }

    IEnumerator StopRecoil2()
    {
        yield return new WaitForSeconds(0.2f);
        combined.Dayandir();
    }

    IEnumerator NotStopShootingWhenOne() 
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("shooting", false);
    }

    private void Reload()
    {
        if (AK47)
        {
            if (bulletCount < 30)
            {
                animator.SetBool("empty", false);
                isReloading = true;
                animator.SetBool("reload", true);
                StartCoroutine(reload());
                //audioSource.PlayOneShot(reloadSound, 1f);
                AudioSource.PlayClipAtPoint(reloadSound, gameObject.transform.position, 0.05f);
            }
        }
        else if (Sniper)
        {
            if (bulletCountSniper < 10)
            {
                animator.SetBool("empty", false);
                isReloading = true;
                animator.SetBool("reload", true);
                StartCoroutine(reloadSniper());
                //audioSource.PlayOneShot(reloadSound, 1f);
                AudioSource.PlayClipAtPoint(reloadSoundSniper, gameObject.transform.position, 1f);
            }
        }
        
    }
    IEnumerator reload()
    {
        yield return new WaitForSeconds(2.7f);
        bulletCount = 30;
        isEmpty = false;
        isReloading = false;
        animator.SetBool("reload", false);  // Ensure shooting state is reset after reloading
    }

    IEnumerator reloadSniper()
    {
        yield return new WaitForSeconds(2.7f);
        bulletCountSniper = 10;
        isEmptySniper = false;
        isReloading = false;
        animator.SetBool("reload", false);  // Ensure shooting state is reset after reloading
    }


    private void ProcessRaycast()
    {
       if (AK47)
        {
            RaycastHit hit;
            if (Physics.Raycast(weaponHead.transform.position, weaponHead.transform.forward, out hit, shootRange))
            {
                // ... existing code ...
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
                        GameObject bloodGo = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));

                        Destroy(bloodGo, 1f);
                        // Convert the hit point to a screen position and show the X hit effect
                        ShowXHitEffectAtPosition(hit.point);
                    }
                }
            }
        }
       else if (Sniper)
        {
            RaycastHit hit;
            if (Physics.Raycast(sniperHead.transform.position, sniperHead.transform.forward, out hit, shootRange))
            {
                // ... existing code ...
                Debug.Log(hit.transform.tag);
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impact);
                }

                GameObject impactGO = Instantiate(impactEffectSniper, hit.point, Quaternion.LookRotation(hit.normal));

                Destroy(impactGO, 2f);

                // Check if hit an enemy
                if (hit.transform.CompareTag("Enemy")) // Ensure your enemy GameObjects have the tag "Enemy"
                {
                    Enemy enemy = hit.transform.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(100); // Decrease health by 20
                        GameObject bloodGo = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));

                        Destroy(bloodGo, 1f);
                        // Convert the hit point to a screen position and show the X hit effect
                        ShowXHitEffectAtPosition(hit.point);
                    }
                }
            }
        }
    }
    private void ShowXHitEffectAtPosition(Vector3 worldPosition)
    {
        Vector2 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        // Move the X hit effect UI element to the calculated screen position
        xHitEffectUI.gameObject.SetActive(true);
        xHitEffectUI.position = screenPosition;

        // Optionally, you can start a coroutine to hide this effect after some time
        StartCoroutine(HideXHitEffect());
    }
    IEnumerator HideXHitEffect()
    {
        yield return new WaitForSeconds(0.2f); // Duration for which the effect is shown, adjust as needed
        xHitEffectUI.gameObject.SetActive(false);
    }
}