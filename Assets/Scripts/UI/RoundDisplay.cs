using UnityEngine;
using TMPro;

public class RoundDisplay : MonoBehaviour
{
    // Reference to the TextMeshPro UI Text element
    public TextMeshProUGUI roundText;

    private void Update()
    {
        // Update the round display every frame
        if (roundText != null && RoundManager.Instance != null)
        {
            // Get the current round from the RoundManager
            int currentRound = RoundManager.Instance.currentRound;

            // Update the text to show the current round
            roundText.text = $"Round: {currentRound}";
        }
    }
}