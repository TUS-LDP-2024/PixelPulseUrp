using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.5f; // Duration of the shake
    public float shakeMagnitude = 0.1f; // Intensity of the shake
    public float dampingSpeed = 1.0f; // Speed at which the shake dampens

    private Transform cameraRoot; // Reference to the PlayerCameraRoot
    private Vector3 initialLocalPosition; // Initial local position of the camera root
    private float shakeElapsedTime = 0f; // Time remaining for the shake

    private void Start()
    {
        // Find the PlayerCameraRoot in the scene
        cameraRoot = GameObject.Find("PlayerCameraRoot")?.transform;
        if (cameraRoot == null)
        {
            Debug.LogError("PlayerCameraRoot not found in the scene! Please ensure the GameObject is named correctly.");
            return;
        }

        // Store the initial local position of the camera root
        initialLocalPosition = cameraRoot.localPosition;
        Debug.Log("CameraShake initialized. Initial local position: " + initialLocalPosition);
    }

    private void Update()
    {
        // If the shake effect is active
        if (shakeElapsedTime > 0)
        {
            Debug.Log("Shake is active. Elapsed time: " + shakeElapsedTime);

            // Calculate the shake offset in world space
            Vector3 worldOffset = Random.insideUnitSphere * shakeMagnitude;

            // Convert the world offset to local space relative to the parent
            Vector3 localOffset = cameraRoot.parent.InverseTransformDirection(worldOffset);

            // Apply the local offset to the camera root's local position
            cameraRoot.localPosition = initialLocalPosition + localOffset;

            Debug.Log("Camera root local position offset: " + localOffset);

            // Reduce the shake time
            shakeElapsedTime -= Time.deltaTime * dampingSpeed;
        }
        else if (shakeElapsedTime > -1f) // Ensure we only reset once
        {
            // Reset the camera root's local position when the shake is over
            cameraRoot.localPosition = initialLocalPosition;
            Debug.Log("Shake ended. Camera root local position reset to: " + initialLocalPosition);
            shakeElapsedTime = -1f; // Prevent repeated resets
        }
    }

    // Trigger the camera shake
    public void TriggerShake()
    {
        Debug.Log("TriggerShake called. Starting shake.");
        shakeElapsedTime = shakeDuration;
    }
}