/*
 * Written by Sam Arutyunyan, 2014 for 'Whiskers of Death Game' project. 
 * enemy cat AI. simple straight forward movement and attack. 
*/
using UnityEngine;
using System.Collections;

public class EnemyCat : MonoBehaviour 
{
    bool alive = true;
    Health health;
    public SkinnedMeshRenderer meshDisplay;
    public Transform target;
    CharacterController characterController;
    public float moveSpeed = 5;
    public float attackDistance = 3;
    public string walkAnim;
    public string attackAnim;
    float attackTimer = 2;

    public int wave;
    bool kitActive = false;

    public HP_GUI playerHP;
	// Use this for initialization
	void Start () 
    {
        health = GetComponent<Health>();
        characterController = GetComponent<CharacterController>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (wave == 0) kitActive = true;
	}
	
	// Update is called once per frame
	void Update () 
    {
        // for debugging..
        switch (wave)
        { 
            case 1:
                if (Input.GetKeyDown(KeyCode.Alpha8))
                    kitActive = true;
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.Alpha9))
                    kitActive = true;
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.Alpha0))
                    kitActive = true;
                break;
        }

        if (kitActive)
        {
            if (health.hp <= 0) alive = false;

            if (alive)
            {
                ProcessMovement();
            }
            else
            {
                meshDisplay.enabled = false;
                transform.FindChild("ParticleBurst").gameObject.SetActive(true);
                ProcessDeath();
            }
        }
	}


    void ProcessMovement()
    {
        Vector3 moveVector = target.position - transform.position;

        if (moveVector.magnitude > attackDistance)
        {
            animation.CrossFade(walkAnim);
            moveVector.Normalize();
            moveVector *= moveSpeed * Time.deltaTime;
            moveVector = new Vector3(moveVector.x, -5, moveVector.z);
            if (wave != 0) characterController.Move(moveVector);
            
        }
        else // close enough, dont move
        {
            attackTimer -= Time.deltaTime;
            animation.CrossFade(attackAnim);
            if (attackTimer <= 0)
            { 
                // do damage
                playerHP.currentHP -= 10;
                attackTimer = 2;
            }
        }

        moveVector = new Vector3(moveVector.x, 0, moveVector.z);
        transform.rotation = Quaternion.LookRotation(moveVector, Vector3.up);
    }

    void ProcessDeath()
    { 
        // waits until particle plays out before destroying itself.
        if (!transform.FindChild("ParticleBurst").GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    { 
        
    }
}

