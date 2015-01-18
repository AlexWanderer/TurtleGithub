/*
 * Written by Sam Arutyunyan, 2014 for 'Whiskers of Death Game' project. 
 * bullet object that is fired by a gun. deactivates after lifetime is up. 
 * based on bullet pooling system (from BulletManager.cs). bullets are recycled
 * rather than destroyed and recreated. 
*/
using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
    float lifeTime = 5;         // life time in seconds
    public bool alive = true;
    Transform hitLocation;
    public float damage = 10;   // how much damage it causes
    float trailTimer = .1f;
	
	// Update is called once per frame
	void Update ()
    {
        trailTimer -= Time.deltaTime;
        //Debug.DrawRay(transform.position, transform.forward * 10, Color.blue);
        if (alive)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0)
            {
                Deactivate();
                lifeTime = 5;
            }
        }
	}

    void OnCollisionEnter (Collision collision) 
    {
        Health hp = collision.transform.GetComponent<Health>();

        if (hp)
        {
            hp.OnDamage(damage);
        }       	   
           
        Deactivate();
        ContactPoint contact = collision.contacts[0];
        transform.position = contact.point;	    
    }

    void Deactivate()
    {
        alive = false;
        transform.collider.enabled = false;
        gameObject.SetActive(false);
        transform.GetComponent<TrailRenderer>().time = 0;       
        trailTimer = .1f;
    }

    //called from BulletManager
    public void Activate(Vector3 positionQ, Quaternion rotationQ, float damage)
    {
        //equipvalent of instantiating
        transform.position = positionQ;
        transform.rotation = rotationQ;
        alive = true;
        rigidbody.velocity = transform.forward * 20;
        damage = this.damage;
      
        gameObject.SetActive(true);
        collider.enabled = true;
        Invoke("trailtrail", .05f); // a couple frames MUST pass in order for trailrenderer time to reset. 

         GetComponent<TrailRenderer>().enabled = true;
    }
}
