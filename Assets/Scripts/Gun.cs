using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Gun : MonoBehaviour {

    public GameObject end, start; // The gun start and end point
    public GameObject gun;
    public GameObject end1, start1; // The handgun start and end point
    public GameObject gun1;
    public Animator animator;
    
    public GameObject spine;
    public GameObject handMag;
    public GameObject gunMag;

    float gunShotTime = 0.1f;
    float gunReloadTime = 1.0f;
    Quaternion previousRotation;
    public float health = 100;
    public bool isDead;
 

    public Text magBullets;
    public Text remainingBullets;
    public Text remainingHealth;

    int magBulletsVal = 30;
    int remainingBulletsVal = 90;
    int magSize = 30;
    public GameObject headMesh;
    public static bool leftHanded { get; private set; }

    public GameObject bulletHole;
    public GameObject muzzleFlash;
    public GameObject shotSound;

    public bool isShortGun;

    // Use this for initialization
    void Start() {
        headMesh.GetComponent<SkinnedMeshRenderer>().enabled = false; // Hiding player character head to avoid bugs :)
        isShortGun = false;
        gun.SetActive(true);
        gun1.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
        // Cool down times
        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }
        if (gunReloadTime >= 0.0f)
        {
            gunReloadTime -= Time.deltaTime;
        }


        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && gunShotTime <= 0 && gunReloadTime <= 0.0f && magBulletsVal > 0 && !isDead)
        { 
            shotDetection(); // Should be completed

            addEffects(); // Should be completed

            animator.SetBool("fire", true);
            gunShotTime = 0.5f;
            
            // Instantiating the muzzle prefab and shot sound
            
            magBulletsVal = magBulletsVal - 1;
            if (magBulletsVal <= 0 && remainingBulletsVal > 0)
            {
                animator.SetBool("reloadAfterFire", true);
                gunReloadTime = 2.5f;
                Invoke("reloaded", 2.5f);
            }
        }
        else
        {
            animator.SetBool("fire", false);
        }

        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.R)) && gunReloadTime <= 0.0f && gunShotTime <= 0.1f && remainingBulletsVal > 0 && magBulletsVal < magSize && !isDead )
        {
            animator.SetBool("reload", true);
            gunReloadTime = 2.5f;
            Invoke("reloaded", 2.0f);
        }
        else if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Q))) // Switch weapon
        {
            animator.SetBool("switchGun", true);
            if (isShortGun) 
            {
                gun.SetActive(true);
                gun1.SetActive(false);
            }
            else 
            {
                gun.SetActive(false);
                gun1.SetActive(true);
            }
            isShortGun = !isShortGun;
            Invoke("switched", 1.5f);
        }
        else
        {
            animator.SetBool("reload", false);
        }
        updateText();
       
    }

    public void Being_shot(float damage) // getting hit from enemy
    {
        health = health - damage;
        // print("Health: ");
        // print(health);
        if (health <= 0)
        {
            isDead = true;

            GetComponent<Animator>().SetBool("dead", true);
            GetComponent<CharacterMovement>().isDead = true;
            headMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;

            print("Game will restart in 10 seconds");
            Invoke("restart", 10.0f); // Restart after 10s
        }
    }

    public void ReloadEvent(int eventNumber) // appearing and disappearing the handMag and gunMag
    {
        if (eventNumber == 1)
        {
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        else if (eventNumber == 2)
        {
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
    }

    void reloaded()
    {
        int newMagBulletsVal = Mathf.Min(remainingBulletsVal + magBulletsVal, magSize);
        int addedBullets = newMagBulletsVal - magBulletsVal;
        magBulletsVal = newMagBulletsVal;
        remainingBulletsVal = Mathf.Max(0, remainingBulletsVal - addedBullets);
        animator.SetBool("reloadAfterFire", false);
    }

    void updateText()
    {
        magBullets.text = magBulletsVal.ToString() ;
        remainingBullets.text = remainingBulletsVal.ToString();
        remainingHealth.text = health.ToString();
    }

    void shotDetection() // Detecting the object which player shot 
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit rayHit;
        if (Physics.Raycast(end.transform.position, (end.transform.position - start.transform.position).normalized, out rayHit, 100.0f, layerMask))
        {
            if (rayHit.transform.tag == "enemy_head")
            {
                print("headshot");
                rayHit.transform.root.transform.GetComponent<enemy>().Being_shot(100.0f); // 100 damage for headshot
            }
            else if (rayHit.transform.tag == "enemy_hand")
            {
                print("handshot");
                rayHit.transform.root.transform.GetComponent<enemy>().Being_shot(10.0f); // 10 damage for hand
            }
            else if (rayHit.transform.tag == "enemy_leg")
            {
                print("legshot");
                rayHit.transform.root.transform.GetComponent<enemy>().Being_shot(20.0f); // 20 damage for leg
            }
            else if (rayHit.transform.tag == "enemy_chest")
            {
                print("chestshot");
                rayHit.transform.root.transform.GetComponent<enemy>().Being_shot(30.0f); // 30 damage for chest
            }
            else if (rayHit.transform.tag == "enemy")
            {
                print("shot");
                rayHit.transform.root.transform.GetComponent<enemy>().Being_shot(10.0f); // 30 damage for chest
            }
            else if (rayHit.transform.tag == "Untagged")
            {
                // bullet hole
                GameObject bulletHoleObj = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                // Holes disappear after 5s
                Destroy(bulletHoleObj, 5.0f);
            }
        }
    }

    void addEffects() // Adding muzzle flash, shoot sound and bullet hole on the wall
    {
        GameObject muzzleFlashObj = Instantiate(muzzleFlash, end.transform.position, end.transform.rotation);
        muzzleFlashObj.GetComponent<ParticleSystem>().Play();
        Destroy(muzzleFlashObj, 1.0f);
        GameObject shotSoundObj = Instantiate(shotSound, transform.position, transform.rotation);
        //shot sound disappear after 1s
        Destroy(shotSoundObj, 1.0f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "door") 
        {
            print("Game will restart in 10 seconds");
            Invoke("restart", 10.0f); // Restart after 10s
        }
        else if (col.gameObject.tag == "ammo")
        {
            print("Find ammo box");
            remainingBulletsVal = 90;
        }
    }

    void restart()
    {
        SceneManager.LoadScene(0);
        print("Game restart");
    }

    void switched()
    {
        animator.SetBool("switchGun", false);
    }
}
