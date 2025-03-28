using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
    public float dampingSpeed = 1.0f;

    private Transform cameraRoot;
    private Vector3 initialLocalPosition;
    private float shakeElapsedTime = 0f;

    private void Start()
    {
        cameraRoot = GameObject.Find("PlayerCameraRoot")?.transform;
        if (cameraRoot == null)
        {
            return;
        }

        initialLocalPosition = cameraRoot.localPosition;
    }

    private void Update()
    {
        if (shakeElapsedTime > 0)
        {
            Vector3 worldOffset = Random.insideUnitSphere * shakeMagnitude;
            Vector3 localOffset = cameraRoot.parent.InverseTransformDirection(worldOffset);
            cameraRoot.localPosition = initialLocalPosition + localOffset;
            shakeElapsedTime -= Time.deltaTime * dampingSpeed;
        }
        else if (shakeElapsedTime > -1f)
        {
            cameraRoot.localPosition = initialLocalPosition;
            shakeElapsedTime = -1f;
        }
    }

    public void TriggerShake()
    {
        shakeElapsedTime = shakeDuration;
    }
}