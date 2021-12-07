using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public Animator animator;
    public GameObject[] targets;
    int index = 0;
    public GameObject player;
    public bool find_player = false;
    public bool being_shot = false;
    public bool isDead = false;
    public float shoot_time = 0.2f;
    public GameObject end, start; // The gun start and end point
    public GameObject bulletHole;
    public GameObject muzzleFlash;
    public GameObject shotSound;
    public float health = 100;
    public GameObject gun;
    public GameObject gun_ray;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetBool("dead", false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v1 = new Vector3((player.transform.position.x - transform.position.x), (player.transform.position.y - transform.position.y), (player.transform.position.z - transform.position.z));
        Vector3 v2 = transform.forward;
        float angle = Vector3.Angle(v1, v2);

        if (isDead == false)
        {
            bool isSaw = (angle <= 20.0f) && (Vector3.Distance(transform.position, player.transform.position) <= 11.0f);
            if ( (player.GetComponent<CharacterMovement>().isDead == false) && (isSaw || (being_shot == true) || (find_player == true)) )
            {
                if (find_player == false)
                {
                    find_player = true;
                }
                // Turn to player and aim the player
                Vector3 aim_player_pos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(aim_player_pos - transform.position), Time.deltaTime*0.5f);
                
                // Fire in 10 meters
                if (Vector3.Distance(transform.position, player.transform.position) <= 10.0f)
                {
                    // shot (5 bullets per second)
                    shoot_time = shoot_time + Time.deltaTime;
                    if (shoot_time >= 0.2f)
                    {
                        GetComponent<Animator>().SetBool("fire", true); 
                        shotDetection();
                        addEffects();
                        shoot_time = 0.0f;
                    }
                }
                else
                {
                    GetComponent<Animator>().SetBool("fire", false); 
                }
            }
            else if (Vector3.Distance(transform.position, player.transform.position) > 11.0f || being_shot == false)
            {
                GetComponent<Animator>().SetBool("fire", false); 
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targets[index].transform.position - transform.position), Time.deltaTime);
                if (Vector3.Distance(targets[index].transform.position, transform.position) <= 0.5f)
                {
                    index = (index + 1) % targets.Length;
                }
            }
        }
    }

    public void Being_shot(float damage) // getting hit from player
    {
        being_shot = true;
        health = health - damage;
        print("Enemy's health: ");
        print(health);
        if (health <= 0)
        {
            isDead = true;
            GetComponent<Animator>().SetBool("dead", true);
            if (gun.transform.parent)
            {
                // give gun rigid body and collider
                Rigidbody rigidbody = gun.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.position = new Vector3(gun.transform.position.x, 0.5f, gun.transform.position.z);

                BoxCollider collider = gun.AddComponent<BoxCollider>();
                collider.size = new Vector3(0.047f, 0.27f, 1.02f);
                collider.center = new Vector3(gun.transform.position.x, 0.5f, gun.transform.position.z);
                float gun_position_x = gun.transform.position.x;
                float gun_position_z = gun.transform.position.z;

                // make the gun independent
                gun.transform.SetParent(null, true);
                gun.transform.position = new Vector3(gun_position_x, 0.5f, gun_position_z);

                // Get rid of green ray from enemy
                gun_ray.transform.SetParent(null, true);
                gun_ray.transform.position = new Vector3(0.0f, -5.0f, 0.0f);
            }
        }
    }

    void shotDetection() // Detecting the object which player shot 
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit rayHit;
        int randomHit = Random.Range(0, 100);
        if (Physics.Raycast(end.transform.position, (end.transform.position - start.transform.position).normalized, out rayHit, 100.0f, layerMask))
        {
            if (randomHit <= 20 && rayHit.transform.tag == "Player")
            {
                // player health minus 10
                rayHit.transform.GetComponent<Gun>().Being_shot(10.0f);
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
}
