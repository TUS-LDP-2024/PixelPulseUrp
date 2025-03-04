using UnityEngine;
using TMPro;

public class ItemPickupPrompt : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "Item"; // Custom name for the item
    public bool useCost = false;     // Toggle to use cost
    public int cost = 0;             // Cost value if applicable

    [Header("UI Settings")]
    public TextMeshProUGUI promptText; // Assign the UI text element here

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowPrompt(false);
        }
    }

    private void ShowPrompt(bool show)
    {
        if (promptText == null)
            return; // Ensure a UI text element is assigned

        if (show)
        {
            // Choose the prompt based on whether a cost is used
            string message = useCost ?
                $"Press 'E' to purchase {itemName} for ${cost}?" :
                $"Press 'E' to pick up {itemName}?";
            promptText.text = message;
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }
}
