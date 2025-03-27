using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;
using System.Collections;

public class VRPlayerController : MonoBehaviour
{
    public GameObject leftGun;   // Gun model for the left hand
    public GameObject rightGun;  // Gun model for the right hand
    public Transform leftHandSocket;  // Socket for left hand to hold the gun
    public Transform rightHandSocket; // Socket for right hand to hold the gun

    public float speed = 5f;          // Movement speed
    public float sprintMultiplier = 2f; // Sprint speed multiplier
    public float rotationAngle = 30f; // Snap rotation angle
    public Transform vrCamera;        // VR Camera reference
    public CharacterController controller; // Character controller

    private Vector2 leftJoystickInput;
    private Vector2 rightJoystickInput;
    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;
    private bool canRotate = true;

    private float gravity = 9.81f;    // Gravity force
    private float verticalVelocity = 0f; // Track falling speed
    private float jumpHeight = 1.5f; // Optional jump height (if needed)

    public GameObject Bullet;
    public Transform leftMuzzleSpawn;  // Muzzle spawn for left hand gun
    public Transform rightMuzzleSpawn; // Muzzle spawn for right hand gun
    public GameObject muzzleflash;
    public bool leftGunCanFire = true;
    public bool rightGunCanFire = true;
    public float FireRate = 0.5f;
    public AudioSource shootsound;
    public AudioClip clip;

    private void Start()
    {
        // Ensure guns are initially deactivated
        leftGun.SetActive(false);
        rightGun.SetActive(false);

        // Initialize character controller
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
            if (controller == null)
            {
                Debug.LogError("❌ Missing CharacterController on Player!");
                return;
            }
        }

        InitializeXRDevices();
    }

    private void Update()
    {
        if (!leftHandDevice.isValid || !rightHandDevice.isValid)
        {
            InitializeXRDevices(); // Re-initialize if VR controllers are not detected
        }

        if (controller.isGrounded && rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool jumpPressed) && jumpPressed)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * 2 * gravity);
        }

        HandleGunDisplay();
        HandleGunShooting();
        HandleVRMovement();
        HandleVRRotation();
        ApplyGravity();
    }

    private void InitializeXRDevices()
    {
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        if (leftHandDevices.Count > 0)
        {
            leftHandDevice = leftHandDevices[0];
            Debug.Log("✅ Left Controller Connected");
        }

        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count > 0)
        {
            rightHandDevice = rightHandDevices[0];
            Debug.Log("✅ Right Controller Connected");
        }
    }

    private void HandleVRMovement()
    {
        if (!leftHandDevice.isValid) return;

        // Check if the left grip button is pressed to enable sprinting
        bool isSprinting = false;
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed) && gripPressed)
        {
            isSprinting = true; // Enable sprinting
        }

        if (leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftJoystickInput))
        {
            Vector3 moveDirection = (vrCamera.forward * leftJoystickInput.y + vrCamera.right * leftJoystickInput.x);
            moveDirection.y = 0; // Prevent flying

            // Adjust speed based on whether the player is sprinting or not
            float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

            if (moveDirection.magnitude > 0.1f)
            {
                controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleVRRotation()
    {
        if (!rightHandDevice.isValid) return;

        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightJoystickInput))
        {
            if (canRotate)
            {
                if (rightJoystickInput.x > 0.5f)
                {
                    transform.Rotate(0, rotationAngle, 0);
                    canRotate = false;
                }
                else if (rightJoystickInput.x < -0.5f)
                {
                    transform.Rotate(0, -rotationAngle, 0);
                    canRotate = false;
                }
            }
            else if (Mathf.Abs(rightJoystickInput.x) < 0.2f)
            {
                canRotate = true; // Reset rotation lock when joystick is centered
            }
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; // Small downward force to stay grounded
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; // Apply gravity
        }

        Vector3 gravityMove = new Vector3(0, verticalVelocity * Time.deltaTime, 0);
        controller.Move(gravityMove);
    }

    private void HandleGunDisplay()
    {
        // Handle left hand grip button press for gun display
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool leftGripPressed) && leftGripPressed)
        {
            if (!leftGun.activeSelf) // If gun is not already active, enable it
            {
                leftGun.SetActive(true);
                leftGun.transform.SetParent(leftHandSocket); // Attach gun to hand
            }
        }
        else
        {
            if (leftGun.activeSelf) // If grip button is released, deactivate gun
            {
                leftGun.SetActive(false);
            }
        }

        // Handle right hand grip button press for gun display
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool rightGripPressed) && rightGripPressed)
        {
            if (!rightGun.activeSelf) // If gun is not already active, enable it
            {
                rightGun.SetActive(true);
                rightGun.transform.SetParent(rightHandSocket); // Attach gun to hand
            }
        }
        else
        {
            if (rightGun.activeSelf) // If grip button is released, deactivate gun
            {
                rightGun.SetActive(false);
            }
        }
    }

    private void HandleGunShooting()
    {
        // Handle shooting for left hand gun
        if (leftGun.activeSelf && leftHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool leftTriggerPressed) && leftTriggerPressed)
        {
            ShootGun(leftGun, leftMuzzleSpawn, true); // Pass true for left gun
        }

        // Handle shooting for right hand gun
        if (rightGun.activeSelf && rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTriggerPressed) && rightTriggerPressed)
        {
            ShootGun(rightGun, rightMuzzleSpawn, false); // Pass false for right gun
        }
    }


    private void ShootGun(GameObject gun, Transform muzzleSpawn, bool isLeftGun)
    {
        // Trigger shooting animation
        gun.GetComponent<Animator>().SetTrigger("Shoot");

        // Start shooting only if the specific gun's CanFire flag is true
        if ((isLeftGun && leftGunCanFire) || (!isLeftGun && rightGunCanFire))
        {
            // Disable firing until cooldown is complete
            if (isLeftGun)
                leftGunCanFire = false;
            else
                rightGunCanFire = false;

            // Start the fire coroutine
            StartCoroutine(Fire(gun, muzzleSpawn, isLeftGun));
        }
    }


    // Fire the bullet and create the muzzle flash
    IEnumerator Fire(GameObject gun, Transform muzzleSpawn, bool isLeftGun)
    {
        // Instantiate the bullet
        GameObject newBullet = Instantiate(Bullet, muzzleSpawn.position, muzzleSpawn.rotation);

        // Set the bullet as a child of muzzle spawn
        newBullet.transform.SetParent(muzzleSpawn);

        // Instantiate the muzzle flash and set it as a child of muzzle spawn
        GameObject muzzleFlashInstance = Instantiate(muzzleflash, muzzleSpawn.position, muzzleSpawn.rotation);
        muzzleFlashInstance.transform.SetParent(muzzleSpawn);

        // Add force to the bullet to make it move
        Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(-muzzleSpawn.right * 1000);  // Apply force in the direction of muzzleSpawn's forward vector
        }

        // Play the shooting sound
        shootsound.PlayOneShot(clip);

        // Wait for the fire rate cooldown before firing again
        yield return new WaitForSeconds(FireRate);

        // Reset the appropriate CanFire flag after cooldown
        if (isLeftGun)
            leftGunCanFire = true;
        else
            rightGunCanFire = true;

        // Destroy the muzzle flash after a brief delay to avoid clutter
        Destroy(muzzleFlashInstance, 0.5f);  // Adjust the destroy time as needed
    }
}
