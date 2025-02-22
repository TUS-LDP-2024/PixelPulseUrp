using UnityEngine;
using TMPro; // Add this namespace for TextMeshPro

public class RoundDisplay : MonoBehaviour
{
    // Reference to the TextMeshPro UI Text element
    public TextMeshProUGUI roundText;

    private void Update()
    {
        // Update the round display every frame
        if (roundText != null)
        {
            // Get the current round from the ZombieSpawner script
            int currentRound = ZombieSpawner.currentRound;

            // Update the text to show the current round
            roundText.text = $"Round: {currentRound}";
        }
    }
}