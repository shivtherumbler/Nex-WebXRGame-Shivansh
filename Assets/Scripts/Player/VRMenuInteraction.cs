using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

public class VRMenuInteraction : MonoBehaviour
{
    public XRRayInteractor rayInteractor;  // Ray Interactor for detecting button
    public float pressDelay = 0.5f;  // Delay before triggering button press
    private float pressTimer = 0f;    // Timer to track the press delay
    private GameObject currentButton; // Currently hovered button
    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    // Update is called once per frame
    void Update()
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult hit))
        {
            if (hit.gameObject != null && hit.gameObject.CompareTag("UIButton"))
            {
                if (currentButton != hit.gameObject)
                {
                    currentButton = hit.gameObject;
                    pressTimer = 0f; // Reset press timer when hovering over a new button
                    Debug.Log("Hovering over button: " + currentButton.name);
                }

                pressTimer += Time.deltaTime;

                // Check if the button is held long enough (gaze or press timer)
                if (pressTimer >= pressDelay || (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTriggerPressed) && rightTriggerPressed) || (leftHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool leftTriggerPressed) && leftTriggerPressed)) // Change to the correct button mapping
                {
                    Debug.Log("Button press detected on: " + currentButton.name);
                    Button button = currentButton.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.Invoke();  // Trigger the button click event
                    }
                    else
                    {
                        Debug.LogError("Button component missing on: " + currentButton.name);
                    }

                    pressTimer = 0f; // Reset press timer
                }
            }
        }
        else
        {
            currentButton = null;  // Reset button if no UI element is hit
            pressTimer = 0f;  // Reset press timer if not hovering over a button
        }
    }
}
