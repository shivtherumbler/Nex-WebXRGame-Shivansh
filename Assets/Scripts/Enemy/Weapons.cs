using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public GameObject Bullet;
    public Transform muzzlespawn;
    public GameObject muzzleflash;
    public bool CanFire;
    public float FireRate = 0.1f;
    public AudioSource shootsound;
    public AudioClip clip;


    private void OnEnable()
    {
        CanFire = true;
        // You can start the shooting automatically or trigger it with a condition
        StartCoroutine(AutoShoot());  // For automatic shooting, you can set a delay between shots
    }

    // Coroutine to handle automatic shooting (if needed)
    IEnumerator AutoShoot()
    {
        while (true)
        {
            if (CanFire)
            {
                Shoot(); // Call the Shoot function
            }
            yield return new WaitForSeconds(0.2f);  // Adjust the shooting delay
        }
    }

    // This function will be called to shoot
    public void Shoot()
    {
        if (CanFire)
        {
            CanFire = false;
            StartCoroutine(Fire());
        }
    }

    // Fire the bullet and create the muzzle flash
    IEnumerator Fire()
    {

        // Instantiate the bullet
        GameObject newBullet = Instantiate(Bullet, muzzlespawn.position, muzzlespawn.rotation);

        // Set the bullet as a child of muzzlespawn
        newBullet.transform.SetParent(muzzlespawn);

        // Instantiate the muzzle flash and set it as a child of muzzlespawn
        GameObject muzzleFlashInstance = Instantiate(muzzleflash, muzzlespawn.position, muzzlespawn.rotation);
        muzzleFlashInstance.transform.SetParent(muzzlespawn);

        // Add force to the bullet to make it move
        Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(-muzzlespawn.right * 1000);  // Apply force in the direction of muzzlespawn's forward vector
        }

        // Play the shooting sound
        shootsound.PlayOneShot(clip);

        // Wait for the fire rate cooldown before firing again
        yield return new WaitForSeconds(FireRate);

        // Set CanFire to true to allow another shot
        CanFire = true;

        // Destroy the muzzle flash after a brief delay to avoid clutter
        Destroy(muzzleFlashInstance, 0.1f);  // Adjust the destroy time as needed
    }

    // Reload function (if needed in the future)
    public void Reload()
    {
        // You can implement the reload functionality here
    }
}
