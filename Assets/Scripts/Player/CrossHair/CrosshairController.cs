using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    // Assign your 4 crosshair UI Image components in the Inspector
    public Image topImage;
    public Image bottomImage;
    public Image leftImage;
    public Image rightImage;

    // Define the colors
    public Color defaultColor = Color.white;
    public Color targetColor = Color.red;

    // How far the raycast should check for objects
    public float rayDistance = 100f;

    void Update()
    {
        // Create a ray from the center of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // If the raycast hits something within the specified distance...
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Check if the object hit is tagged "Enemy"
            if (hit.transform.CompareTag("Enemy"))
            {
                ChangeCrosshairColor(targetColor);
                return;
            }
        }
        // If no enemy is hit, revert to the default color
        ChangeCrosshairColor(defaultColor);
    }

    // Helper method to change the color of all crosshair elements
    void ChangeCrosshairColor(Color color)
    {
        topImage.color = color;
        bottomImage.color = color;
        leftImage.color = color;
        rightImage.color = color;
    }
}
