using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cardPanel;  // Parent panel for all cards

    // Left Card
    public GameObject leftCard;
    public TextMeshProUGUI leftCardTitle;
    public TextMeshProUGUI leftCardDescription;
    public Button leftCardButton;
    public Image leftCardIcon;

    // Middle Card
    public GameObject middleCard;
    public TextMeshProUGUI middleCardTitle;
    public TextMeshProUGUI middleCardDescription;
    public Button middleCardButton;
    public Image middleCardIcon;

    // Right Card
    public GameObject rightCard;
    public TextMeshProUGUI rightCardTitle;
    public TextMeshProUGUI rightCardDescription;
    public Button rightCardButton;
    public Image rightCardIcon;

    private void Start()
    {
        // Initialize all card buttons with empty listeners
        leftCardButton.onClick.AddListener(() => { });
        middleCardButton.onClick.AddListener(() => { });
        rightCardButton.onClick.AddListener(() => { });

        // Hide all cards at start
        cardPanel.SetActive(false);
        leftCard.SetActive(false);
        middleCard.SetActive(false);
        rightCard.SetActive(false);

        Debug.Log("Card UI initialized", this);
    }

    public void ShowRandomUpgrade()
    {
        if (!VerifyComponents()) return;

        // Show the panel and all cards
        cardPanel.SetActive(true);
        leftCard.SetActive(true);
        middleCard.SetActive(true);
        rightCard.SetActive(true);

        // Unlock cursor for selection
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("All cards shown", cardPanel);
    }

    private void OnCardClicked() // Generic click handler for now
    {
        HideAllCards();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Notify RoundManager to resume
        RoundManager.Instance?.ResumeAfterCardSelection();
    }

    private bool VerifyComponents()
    {
        // Verify all card components exist
        bool cardsValid = leftCard != null && middleCard != null && rightCard != null;

        // Verify all buttons exist
        bool buttonsValid = leftCardButton != null &&
                          middleCardButton != null &&
                          rightCardButton != null;

        return cardPanel != null && cardsValid && buttonsValid;
    }
}