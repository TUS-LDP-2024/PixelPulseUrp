using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        // Face the camera, keeping upright orientation
        Vector3 lookPos = transform.position + mainCam.transform.rotation * Vector3.forward;
        Vector3 up = mainCam.transform.rotation * Vector3.up;
        transform.LookAt(lookPos, up);
    }
}
