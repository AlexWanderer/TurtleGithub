/*
 * Written by Sam Arutyunyan, 2014 for 'Whiskers of Death Game' project. 
 * Attach to a gun (pistol, rifle, etc) responds to player input to shoot gun.
 * interacts with gun GUI, and bullet manager
*/
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    //bulletType
    public Rigidbody bulletPrefab;
    float rateOfFire;           // bullets per second?
    public int bulletsPerClip = 12;
    int clips = 20;
    public Transform bulletSpawn;
    Vector3 desiredPosition;
    Quaternion desiredRotation;
    float recoilAmountTotal;    // how far back in z axis it gets pushed. 
    float recoilAmountCurrent;  // how far back it is at the moment. this doesnt get set to anything, just keeps track of amount. 
    float recoilAmount = .03f;  // how much recoil is added to current each frame. 
    public Transform muzzleFlash;
    float timedif = 0;

    float gunBobX;
    float gunBobY;
    float currentGunBobX; 
    float currentGunBobY;

    public Ammo_GUI gui;
    public float fireTimer = .1f;

	// Use this for initialization
	void Start () 
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .4f, 0));    
        
        bulletSpawn.LookAt(bulletSpawn.position + ray.direction);
        bulletSpawn.position += bulletSpawn.forward * .5f;

        muzzleFlash.gameObject.SetActive(false);

        gui.ammoLeft = 180;
        gui.bulletCount = bulletsPerClip;

        // animation["p_recoil"].speed *= 2;
	}
	
	// Update is called once per frame
	void Update () 
    {
        fireTimer -= Time.deltaTime;

        if (!animation.IsPlaying("p_reload"))
        {
            if (Input.GetButton("Fire1"))
            {
                if (fireTimer <= 0)
                {
                    BulletManager.instance.SpawnBullet(bulletSpawn.position, bulletSpawn.rotation, 10f);
                    muzzleFlash.gameObject.SetActive(true);
                    animation.Play("p_recoil");
                    timedif = Time.time;
                    fireTimer = .1f;

                    gui.bulletCount--;
                    if (gui.bulletCount <= 0)
                    {
                        animation.Play("p_reload");
                        gui.ammoLeft -= 12;
                        gui.bulletCount = 12;

                    }
                }
            }
        }

        if (Time.time - timedif > .1f) muzzleFlash.gameObject.SetActive(false);

        if (Input.GetKeyDown(KeyCode.R))
        {
            animation.Play("p_reload");
            gui.ammoLeft -= (bulletsPerClip - gui.bulletCount);
            gui.bulletCount = bulletsPerClip;
        } 
	}
}
