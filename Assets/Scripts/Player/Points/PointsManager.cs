using UnityEngine;
using TMPro; // Add this namespace for TextMeshPro

public class PointsManager : MonoBehaviour
{
    [Header("Points Settings")]
    public int points = 0;              // Current points
    public TextMeshProUGUI pointsText; // UI element to display points (TextMeshPro)

    private void Start()
    {
        UpdatePointsUI();
    }

    // Call this when the player earns points (e.g., from a zombie hit)
    public void AddPoints(int amount)
    {
        points += amount;
        Debug.Log("Points added. Current points: " + points);
        UpdatePointsUI();
    }

    // Use this to spend points (e.g., for opening a door)
    // Returns true if the player had enough points and the points were spent.
    public bool SpendPoints(int amount)
    {
        if (points >= amount)
        {
            points -= amount;
            Debug.Log("Points spent. Current points: " + points);
            UpdatePointsUI();
            return true;
        }
        else
        {
            Debug.Log("Not enough points. Current points: " + points + ", required: " + amount);
        }
        return false;
    }

    // Update the UI text to show current points.
    private void UpdatePointsUI()
    {
        if (pointsText != null)
            pointsText.text = "Points: " + points.ToString();
    }
}